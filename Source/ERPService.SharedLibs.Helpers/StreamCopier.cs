using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Начальная позиция для копирования потоков
    /// </summary>
    public enum SourceStartPosition
    {
        /// <summary>
        /// С начала потока-источника
        /// </summary>
        Beginning,

        /// <summary>
        /// С текущей позиции потока-источника
        /// </summary>
        Current,

        /// <summary>
        /// С определенной позиции потока-источника
        /// </summary>
        Offset
    }

    /// <summary>
    /// Вспомогательный класс для копирования данных одного потока в другой
    /// </summary>
    public class StreamCopier
    {
        #region Поля

        private byte[] _buffer;
        private bool _disposeSource;
        private bool _disposeDest;
        private Int64 _sourceOffset;
        private Int64 _length;
        private SourceStartPosition _startPosition;

        #endregion

        #region Свойства

        /// <summary>
        /// Размер внутреннего буфера данных
        /// </summary>
        public int BufferSize
        {
            get { return _buffer.Length; }
            set 
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("BufferSize");
                _buffer = new byte[value];
            }
        }

        /// <summary>
        /// Освобождать ресурсы потока-источника
        /// </summary>
        /// <remarks>Значение по умолчанию - false</remarks>
        public bool DisposeSource
        {
            get { return _disposeSource; }
            set { _disposeSource = value; }
        }

        /// <summary>
        /// Освобождать ресурсы потока-приемника
        /// </summary>
        /// <remarks>Значение по умолчанию - false</remarks>
        public bool DisposeDest
        {
            get { return _disposeDest; }
            set { _disposeDest = value; }
        }

        /// <summary>
        /// Смещение от начала потока-источника
        /// </summary>
        /// <remarks>Используется, если свойство StartPosition 
        /// установлено в SourceStartPosition.Offset</remarks>
        public Int64 SourceOffset
        {
            get { return _sourceOffset; }
            set 
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("SourceOffset");
                _sourceOffset = value; 
            }
        }

        /// <summary>
        /// Длина копируемых данных
        /// </summary>
        /// <remarks>
        /// Если в момент вызова метода Copy длина копируемых данных равна: 
        /// 1) нулю, то она устанавливается равной длине потока-источника;
        /// 2) -1, то копирование выполняется до конца потока
        /// </remarks>
        public Int64 Length
        {
            get { return _length; }
            set 
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("Length");
                _length = value; 
            }
        }

        /// <summary>
        /// Начальная позиция в потоке-источнике
        /// </summary>
        /// <remarks>Для потоков, не поддерживающих поиск, значение свойства должно быть 
        /// установлено в значение SourceStartPosition.Current</remarks>
        public SourceStartPosition StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        public StreamCopier(int bufferSize)
        {
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException("bufferSize");

            // инициализация свойств начальными значениями
            BufferSize = bufferSize;
            _disposeSource = false;
            _disposeDest = false;
            _sourceOffset = 0;
            _length = 0;
            _startPosition = SourceStartPosition.Beginning;
        }

        /// <summary>
        /// Создает экземпляр класса, задает размер буфера равным 1 Мб
        /// </summary>
        public StreamCopier()
            : this(1048576)
        {
        }

        #endregion

        #region Открытые методы

        /// <summary>
        /// Копирование данных из одного потока в другой
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="dest">Поток-получатель</param>
        /// <returns>Количество скопированных байт</returns>
        public Int64 Copy(Stream source, Stream dest)
        {
            try
            {
                // начальная позиция в потоке-источнике
                switch (_startPosition)
                {
                    case SourceStartPosition.Beginning:
                        if (source.CanSeek)
                            source.Seek(0, SeekOrigin.Begin);
                        break;
                    case SourceStartPosition.Offset:
                        if (source.CanSeek)
                            source.Seek(_sourceOffset, SeekOrigin.Begin);
                        break;
                }

                Int64 totalCopied = 0;

                // длина копируемых данных
                switch (_length)
                {
                    case 0:
                        // равна длине потока источника
                        _length = source.Length;
                        break;
                    case -1:
                        // равна максимально возможному количеству байт
                        // чтение будет выполняться до конца потока-источника
                        _length = Int64.MaxValue;
                        break;
                }

                while (totalCopied < _length)
                {
                    Int64 count = _length - totalCopied;
                    if (count > _buffer.Length)
                        count = _buffer.Length;

                    int bytesRead = source.Read(_buffer, 0, (int)count);
                    if (bytesRead == 0)
                        break;

                    dest.Write(_buffer, 0, bytesRead);
                    totalCopied += bytesRead;
                }

                return totalCopied;
            }
            finally
            {
                if (_disposeSource)
                    source.Dispose();
                if (_disposeDest)
                    dest.Dispose();
            }
        }

        #endregion
    }
}
