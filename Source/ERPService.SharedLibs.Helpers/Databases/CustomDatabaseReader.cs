using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// ������� ��������������� ����� ��� ������ ������ �� ���� ������
    /// </summary>
    public abstract class CustomDatabaseReader : CustomDatabaseHelper
    {
        private String _text;
        private List<KeyValuePair<String, Object>> _parameters;
        private IDbConnection _connection;
        private IDbCommand _command;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomDatabaseReader()
            : base()
        {
            _text = String.Empty;
            _parameters = new List<KeyValuePair<String, Object>>();
        }

        private void StartRead()
        {
            if (String.IsNullOrEmpty(_text))
                throw new ArgumentNullException("Text");

            // ���� ������ �������� ��������, ����������� ������� � ����������
            ReleaseResources();

            // ������� � ������������� ����������
            _connection = GetConnection();
            _connection.Open();

            // ������� � �������������� �������
            _command = _connection.CreateCommand();
            _command.Transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
            _command.CommandText = _text;
            foreach (KeyValuePair<String, Object> kvp in _parameters)
            {
                NewParameter(_command, kvp.Key, kvp.Value);
            }
        }

        private void ReleaseResources()
        {
            if (_command != null)
            {
                if (_command.Transaction != null)
                    _command.Transaction.Dispose();
                _command.Dispose();
                _command = null;
            }
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public override void Dispose()
        {
            ReleaseResources();
            base.Dispose();
        }

        /// <summary>
        /// ����� ������� ������ ������
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// ��������� �������� � ������� ������ ������
        /// </summary>
        /// <param name="parameterName">��� ���������</param>
        /// <param name="parameterValue">�������� ���������</param>
        public void NewParameter(String parameterName, Object parameterValue)
        {
            _parameters.Add(new KeyValuePair<String, Object>(
                parameterName, parameterValue));
        }

        /// <summary>
        /// ������ ��������� ������ �� ���� ������
        /// </summary>
        /// <returns>������ ��� ������ ������</returns>
        public IDataReader ReadMany()
        {
            StartRead();
            return _command.ExecuteReader();
        }

        /// <summary>
        /// ������ ���������� �������� �� ���� ������
        /// </summary>
        /// <typeparam name="TResult">��� ������������� ��������</typeparam>
        /// <returns>��������� �������� ��� null, ���� ������ ������ ������ ���������</returns>
        public Nullable<TResult> ReadOne<TResult>() where TResult : struct
        {
            StartRead();
            Object value = _command.ExecuteScalar();
            return (value == null || value == DBNull.Value) ?
                null : (Nullable<TResult>)(TResult)value;
        }

        /// <summary>
        /// ������ ���������� �������� �� ���� ������
        /// </summary>
        /// <typeparam name="TResult">��� ������������� ��������</typeparam>
        /// <param name="defaultValue">�������� �� ���������</param>
        /// <returns>��������� �������� ��� �������� �� ���������</returns>
        public TResult ReadOne<TResult>(TResult defaultValue) where TResult : struct
        {
            Nullable<TResult> value = ReadOne<TResult>();
            return value == null ? defaultValue : value.Value;
        }
    }
}
