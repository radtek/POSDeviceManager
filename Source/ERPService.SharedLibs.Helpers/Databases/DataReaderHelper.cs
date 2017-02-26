using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// Вспомогательный класс для взаимодействия с IDataReader
    /// </summary>
    public class DataReaderHelper
    {
        private IDataReader _reader;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="reader">IDataReader, подготовленный для чтения данных</param>
        public DataReaderHelper(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            _reader = reader;
        }

        /// <summary>
        /// Чтение логического значение (1 - истина, остальное - ложь)
        /// </summary>
        /// <param name="column">Номер колонки</param>
        public bool GetBoolean(int column)
        {
            return GetInt32(column) == 1;
        }

        /// <summary>
        /// Чтение строкового значения. В случае с DBNull возвращается пустая строка
        /// </summary>
        /// <param name="column">Номер колонки</param>
        public string GetString(int column)
        {
            return _reader.IsDBNull(column) ? string.Empty : _reader.GetString(column);
        }

        /// <summary>
        /// Чтение целочисленного значения. В случае с DBNull возвращается 0
        /// </summary>
        /// <param name="column">Номер колонки</param>
        public int GetInt32(int column)
        {
            return _reader.IsDBNull(column) ? 0 : _reader.GetInt32(column);
        }

        /// <summary>
        /// Чтение даты/времени. В случае с DBNull возвращается DateTime.MinValue
        /// </summary>
        /// <param name="column">Номер колонки</param>
        public DateTime GetDateTime(int column)
        {
            return _reader.IsDBNull(column) ? DateTime.MinValue : _reader.GetDateTime(column);
        }
    }
}
