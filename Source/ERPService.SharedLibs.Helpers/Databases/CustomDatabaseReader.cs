using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// Базовый вспомогательный класс для чтения данных из базы данных
    /// </summary>
    public abstract class CustomDatabaseReader : CustomDatabaseHelper
    {
        private String _text;
        private List<KeyValuePair<String, Object>> _parameters;
        private IDbConnection _connection;
        private IDbCommand _command;

        /// <summary>
        /// Создает экземпляр класса
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

            // если чтение запущено повторно, освобождаем команду и соединение
            ReleaseResources();

            // создаем и устанавливаем соединение
            _connection = GetConnection();
            _connection.Open();

            // создаем и инициализируем команду
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
        /// Освобождение ресурсов
        /// </summary>
        public override void Dispose()
        {
            ReleaseResources();
            base.Dispose();
        }

        /// <summary>
        /// Текст команды чтения данных
        /// </summary>
        public String Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Добавляет параметр в команду чтения данных
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="parameterValue">Значение параметра</param>
        public void NewParameter(String parameterName, Object parameterValue)
        {
            _parameters.Add(new KeyValuePair<String, Object>(
                parameterName, parameterValue));
        }

        /// <summary>
        /// Чтение множества данных из базы данных
        /// </summary>
        /// <returns>Объект для чтения данных</returns>
        public IDataReader ReadMany()
        {
            StartRead();
            return _command.ExecuteReader();
        }

        /// <summary>
        /// Чтение скалярного значения из базы данных
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого значения</typeparam>
        /// <returns>Скалярное значение или null, если запрос вернул пустой результат</returns>
        public Nullable<TResult> ReadOne<TResult>() where TResult : struct
        {
            StartRead();
            Object value = _command.ExecuteScalar();
            return (value == null || value == DBNull.Value) ?
                null : (Nullable<TResult>)(TResult)value;
        }

        /// <summary>
        /// Чтение скалярного значения из базы данных
        /// </summary>
        /// <typeparam name="TResult">Тип возвращаемого значения</typeparam>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Скалярное значение или значение по умолчанию</returns>
        public TResult ReadOne<TResult>(TResult defaultValue) where TResult : struct
        {
            Nullable<TResult> value = ReadOne<TResult>();
            return value == null ? defaultValue : value.Value;
        }
    }
}
