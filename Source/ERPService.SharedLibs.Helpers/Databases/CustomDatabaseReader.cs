using System;
using System.Collections.Generic;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// ������� ��������������� ����� ��� ������ ������ �� ���� ������
    /// </summary>
    public abstract class CustomDatabaseReader : CustomDatabaseHelper
    {
        private string _text;
        private List<KeyValuePair<string, Object>> _parameters;
        private IDbConnection _connection;
        private IDbCommand _command;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomDatabaseReader()
            : base()
        {
            _text = string.Empty;
            _parameters = new List<KeyValuePair<string, Object>>();
        }

        private void StartRead()
        {
            if (string.IsNullOrEmpty(_text))
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
            foreach (KeyValuePair<string, Object> kvp in _parameters)
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
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// ��������� �������� � ������� ������ ������
        /// </summary>
        /// <param name="parameterName">��� ���������</param>
        /// <param name="parameterValue">�������� ���������</param>
        public void NewParameter(string parameterName, Object parameterValue)
        {
            _parameters.Add(new KeyValuePair<string, Object>(
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
