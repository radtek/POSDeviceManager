using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// Базовый вспомогательный класс для работы с базой данных
    /// </summary>
    public abstract class CustomDatabaseHelper : IDisposable
    {
        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        protected CustomDatabaseHelper()
        {
        }

        /// <summary>
        /// Добавляет параметр в команду
        /// </summary>
        /// <param name="command">Команда</param>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="parameterValue">Значение параметра</param>
        /// <returns>Вновь добавленный параметр</returns>
        public IDataParameter NewParameter(IDbCommand command, string parameterName,
            Object parameterValue)
        {
            // создаем новый параметр
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            // добавляем его в коллекцию параметров команды
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Безопасный откат транзакции
        /// </summary>
        /// <param name="transaction">Транзакция</param>
        public void SafeRollback(IDbTransaction transaction)
        {
            if (transaction == null)
                return;

            try
            {
                // откатываем транзакцию
                transaction.Rollback();
            }
            catch (InvalidOperationException)
            {
                // соединение прервана или нечего откатывать
            }
        }

        /// <summary>
        /// Создать подключение к базе данных
        /// </summary>
        /// <returns>Подключение к базе данных</returns>
        protected abstract IDbConnection GetConnection();

        #region Реализация IDisposable

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
