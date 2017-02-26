using System;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// Базовый вспомогательный класс для выполнения команд, требующих 
    /// контекста транзакции
    /// </summary>
    public abstract class CustomDatabaseExecutor : CustomDatabaseHelper
    {
        /// <summary>
        /// Делегат для выполнения команд в контексте транзакции
        /// </summary>
        /// <param name="connection">Подключение к базе</param>
        /// <param name="transaction">Транзакция</param>
        public delegate void ExecutorDelegate(IDbConnection connection,
            IDbTransaction transaction);

        private ExecutorDelegate _executor;

        /// <summary>
        /// Делегат для выполнения команд в контексте транзакции
        /// </summary>
        public ExecutorDelegate Executor
        {
            get { return _executor; }
            set { _executor = value; }
        }

        /// <summary>
        /// Вызывается в случае применения транзакции
        /// </summary>
        protected abstract void OnCommit();

        /// <summary>
        /// Вызывается в случае отката транзакции
        /// </summary>
        /// <param name="ex">Исключение</param>
        protected abstract void OnRollback(Exception ex);

        /// <summary>
        /// Выполнение команд в контексте транзакции
        /// </summary>
        public void Execute()
        {
            if (_executor == null)
                throw new ArgumentNullException("Executor");

            // создаем соединение
            using (IDbConnection connection = GetConnection())
            {
                // устанавливаем его
                connection.Open();
                // начинаем трнанзакцию
                using (IDbTransaction transaction = connection.BeginTransaction(
                    IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // выполняем необходимы команды
                        _executor(connection, transaction);
                        // сохраняем транзакцию
                        transaction.Commit();
                        OnCommit();
                    }
                    catch (Exception ex)
                    {
                        // откат транзакции
                        SafeRollback(transaction);
                        OnRollback(ex);
                        // повторно бросаем исключение
                        throw;
                    }
                }
            }
        }
    }
}
