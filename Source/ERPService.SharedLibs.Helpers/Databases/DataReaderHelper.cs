using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// ��������������� ����� ��� �������������� � IDataReader
    /// </summary>
    public class DataReaderHelper
    {
        private IDataReader _reader;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="reader">IDataReader, �������������� ��� ������ ������</param>
        public DataReaderHelper(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            _reader = reader;
        }

        /// <summary>
        /// ������ ����������� �������� (1 - ������, ��������� - ����)
        /// </summary>
        /// <param name="column">����� �������</param>
        public Boolean GetBoolean(Int32 column)
        {
            return GetInt32(column) == 1;
        }

        /// <summary>
        /// ������ ���������� ��������. � ������ � DBNull ������������ ������ ������
        /// </summary>
        /// <param name="column">����� �������</param>
        public String GetString(Int32 column)
        {
            return _reader.IsDBNull(column) ? String.Empty : _reader.GetString(column);
        }

        /// <summary>
        /// ������ �������������� ��������. � ������ � DBNull ������������ 0
        /// </summary>
        /// <param name="column">����� �������</param>
        public Int32 GetInt32(Int32 column)
        {
            return _reader.IsDBNull(column) ? 0 : _reader.GetInt32(column);
        }

        /// <summary>
        /// ������ ����/�������. � ������ � DBNull ������������ DateTime.MinValue
        /// </summary>
        /// <param name="column">����� �������</param>
        public DateTime GetDateTime(Int32 column)
        {
            return _reader.IsDBNull(column) ? DateTime.MinValue : _reader.GetDateTime(column);
        }
    }
}
