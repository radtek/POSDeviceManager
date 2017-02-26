using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ERPService.SharedLibs.Eventlog.FileLink
{
    /// <summary>
    /// Индексный файл
    /// </summary>
    public class Index : IDisposable
    {
        #region Поля

        private FileStream _stream;
        private BinaryReader _reader;
        private Int64 _recordCount;
        private Int64 _logSize;
        private Mutex _syncIndexMutex;

        #endregion

        #region Вложенные типы

        /// <summary>
        /// Статус индекса
        /// </summary>
        private enum IndexState
        {
            /// <summary>
            /// Валидный
            /// </summary>
            Valid,

            /// <summary>
            /// Поврежденный или не существующий
            /// </summary>
            Corrupted,

            /// <summary>
            /// Устаревший
            /// </summary>
            Obsolete
        }

        /// <summary>
        /// Результат валидации индекса
        /// </summary>
        private class IndexValidationResult
        {
            public IndexState State { get; set; }
            public Int64 RecordsCount { get; set; }
            public Int64 LastRecordOffset { get; set; }
        }

        /// <summary>
        /// Информация о записи
        /// </summary>
        public class RecordInfo
        {
            /// <summary>
            /// Смещение записи от начала файла
            /// </summary>
            public Int64 Offset { get; set; }

            /// <summary>
            /// Число строк в записи
            /// </summary>
            public int LinesCount { get; set; }
        }

        #endregion

        #region Закрытые методы

        /// <summary>
        /// Переиндексация
        /// </summary>
        /// <param name="validationResult">Результат валидации индекса</param>
        /// <param name="logFile">Имя лог-файла</param>
        /// <param name="indexName">Имя индекса</param>
        private void ReIndex(IndexValidationResult validationResult, string logFile,
            string indexName)
        {
            if (validationResult.State == IndexState.Corrupted && File.Exists(indexName))
                // удаляем существующий индекс
                File.Delete(indexName);

            using (var source = OpenForReading(logFile))
            using (var index = OpenForWriting(indexName))
            {
                var recordId = string.Empty;
                var recordOffset = 0L;
                var recordLines = 0;

                var reader = new LineReader(source);

                using (var writer = new BinaryWriter(index, Encoding.Default))
                {
                    // записываем размер исходного лога
                    writer.Seek(0, SeekOrigin.Begin);
                    writer.Write(source.Length);

                    switch (validationResult.State)
                    {
                        case IndexState.Corrupted:
                            // оставляем место для количества записей в исходном логе
                            writer.Write(validationResult.RecordsCount);
                            break;
                        case IndexState.Obsolete:
                            // подсчет числа записей начнем с последней известной 
                            // записи в логе
                            validationResult.RecordsCount--;
                            // перемещаемся к началу последней записи в логе
                            source.Seek(validationResult.LastRecordOffset, SeekOrigin.Begin);
                            // перемещаемся к концу индексного файла минус один элемент индекса
                            // (начинаем переиндексацию с последней известной записи!)
                            writer.Seek(-(int)IndexElementSize, SeekOrigin.End);
                            break;
                    }

                    // читаем исходный лог до конца
                    while (!reader.Eof)
                    {
                        // запоминаем текущую позицию в логе
                        var currentOffset = source.Position;

                        // читаем очередную строку лога
                        var rawEntry = EventRecordHelper.GetRawEntry(reader.ReadLine());
                        // проверяем ее на валидность по числу полей
                        if (!EventRecordHelper.IsValidEntry(rawEntry))
                            // вероятнее всего, это - недозаписанное событие
                            // в текущем логе, прерываем чтение
                            break;

                        // если сохраненный идентификатор записи не совпадает с 
                        // идентификатором считанной строки, эта строка является
                        // первой в записи
                        if (string.Compare(recordId, rawEntry[0]) != 0)
                        {
                            if (!string.IsNullOrEmpty(recordId))
                            {
                                // сведения о предыдущей записи нужно сохранить
                                // в индексном файле
                                writer.Write(recordOffset);
                                writer.Write(recordLines);

                                // обнуляем счетчик строк в записи
                                recordLines = 0;
                            }

                            // увеличиваем счетчик записей в логе
                            validationResult.RecordsCount++;
                            recordId = rawEntry[0];
                            recordOffset = currentOffset;
                        }

                        // увеличиваем число строк в записи
                        recordLines++;

                        // последняя запись
                        if (reader.Eof)
                        {
                            // сохраняем сведения о последней записи
                            writer.Write(recordOffset);
                            writer.Write(recordLines);
                        }
                    }

                    // сохраняем итоговое количество записей в индексе
                    writer.Seek(sizeof(Int64), SeekOrigin.Begin);
                    writer.Write(validationResult.RecordsCount);
                }
            }
        }

        /// <summary>
        /// Валидация индекса
        /// </summary>
        /// <param name="logFile">Имя исходного лог-файла</param>
        /// <param name="indexName">Имя индексного файла</param>
        /// <returns>Результат валидации индекса</returns>
        private IndexValidationResult ValidateIndex(string logFile, string indexName)
        {
            if (!File.Exists(indexName))
                // индекс поврежден, если индексного файла не существует
                return new IndexValidationResult() { State = IndexState.Corrupted };

            // открываем индекс на чтение
            using (var index = OpenForReading(indexName))
            {
                // проверяем его структуру:
                //   размер исходного лога | sizeof(Int64)
                //   число записей | sizeof(Int64)
                //   смещение/количество строк записи 1 | sizeof(Int64) + sizeof(Int32)
                //   смещение/количество строк записи 2
                //   и т.д.
                //
                // т.о. размер индекса должен быть: 
                // sizeof(Int64) * 2 + [число записей] * (sizeof(Int64) + sizeof(Int32))

                // размер индекса не может быть меньше, чем минимально допустимый
                if (index.Length < IndexSize(0))
                    // индекс поврежден
                    return new IndexValidationResult() { State = IndexState.Corrupted };

                // считываем размер исходного лога и число записей
                using (var reader = new BinaryReader(index, Encoding.Default))
                {
                    var sourceLogSize = reader.ReadInt64();
                    var recordsCount = reader.ReadInt64();

                    // проверяем размер индекса по считанному числу записей
                    if (index.Length != IndexSize(recordsCount))
                        // индекс поврежден
                        return new IndexValidationResult() { State = IndexState.Corrupted };

                    // открываем для чтения исходный лог, нужно определить его размер
                    using (var source = OpenForReading(logFile))
                    {
                        if (sourceLogSize != source.Length)
                        {
                            // индекс устарел
                            var validationResult = new IndexValidationResult();
                            validationResult.State = IndexState.Obsolete;
                            validationResult.RecordsCount = recordsCount;
                            // смещение последней записи в логе
                            // с него будет начинаться частичная переиндексация
                            index.Seek(-IndexElementSize, SeekOrigin.End);
                            validationResult.LastRecordOffset = reader.ReadInt64();

                            return validationResult;
                        }
                    }
                }
            }

            // индекс валиден
            return new IndexValidationResult() { State = IndexState.Valid };
        }

        /// <summary>
        /// Считает размер индекса в зависимости от числа записей
        /// </summary>
        /// <param name="recordCount">Число записей</param>
        /// <returns>Размер индекса</returns>
        private Int64 IndexSize(Int64 recordCount)
        {
            return sizeof(Int64) * 2 + recordCount * IndexElementSize;
        }

        private FileStream OpenForReading(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
        }

        private FileStream OpenForWriting(string fileName)
        {
            return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, 
                FileShare.None, 1024, FileOptions.WriteThrough);
        }

        private Int64 IndexElementSize
        {
            get { return sizeof(Int64) + sizeof(int); }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="logFile">Имя исходного лог-файла</param>
        public Index(string logFile)
        {
            if (string.IsNullOrEmpty(logFile))
                throw new ArgumentNullException("logFile");

            _recordCount = -1;
            _logSize = -1;

            // блокируем совместный доступ к индексу
            _syncIndexMutex = MutexHelper.CreateSyncIndexMutex(logFile);
            _syncIndexMutex.WaitMutex();

            // формируем имя индексного файла
            var indexName = string.Format("{0}.index", logFile);
            // проверяем индекс
            var validationResult = ValidateIndex(logFile, indexName);
            if (validationResult.State != IndexState.Valid)
                // выполняем переиндексацию
                ReIndex(validationResult, logFile, indexName);

            // открываем индекс
            _stream = OpenForReading(indexName);
            _reader = new BinaryReader(_stream, Encoding.Default);
        }

        #endregion

        #region Открытые свойства и методы

        /// <summary>
        /// Число записей
        /// </summary>
        public Int64 RecordCount
        {
            get
            {
                if (_recordCount == -1)
                {
                    _stream.Seek(sizeof(Int64), SeekOrigin.Begin);
                    _recordCount = _reader.ReadInt64();
                }
                return _recordCount;
            }
        }

        /// <summary>
        /// Размер лога
        /// </summary>
        public Int64 LogSize
        {
            get
            {
                if (_logSize == -1)
                {
                    _stream.Seek(0, SeekOrigin.Begin);
                    _logSize = _reader.ReadInt64();
                }
                
                return _logSize;
            }
        }

        /// <summary>
        /// Переход к индексу по номеру записи
        /// </summary>
        /// <param name="recordNo">Номер записи</param>
        public void Seek(Int64 recordNo)
        {
            // вычисляем смещение в индексе, по которому находится информация
            // о записи
            var offset = 2 * sizeof(Int64) + recordNo * (sizeof(Int64) + sizeof(int));
            _stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Возвращает индексированную информацию об очередной записи
        /// </summary>
        public RecordInfo GetNext()
        {
            return new RecordInfo()
            {
                Offset = _reader.ReadInt64(),
                LinesCount = _reader.ReadInt32()
            };
        }

        #endregion

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
                _stream = null;
            }
            else
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }

            if (_syncIndexMutex != null)
            {
                _syncIndexMutex.Close();
                _syncIndexMutex = null;
            }
        }

        #endregion
    }
}
