using System;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// ������� ��������������� ����� ��� ���������� ������, ��������� 
    /// ��������� ����������
    /// </summary>
    public abstract class CustomDatabaseExecutor : CustomDatabaseHelper
    {
        /// <summary>
        /// ������� ��� ���������� ������ � ��������� ����������
        /// </summary>
        /// <param name="connection">����������� � ����</param>
        /// <param name="transaction">����������</param>
        public delegate void ExecutorDelegate(IDbConnection connection,
            IDbTransaction transaction);

        private ExecutorDelegate _executor;

        /// <summary>
        /// ������� ��� ���������� ������ � ��������� ����������
        /// </summary>
        public ExecutorDelegate Executor
        {
            get { return _executor; }
            set { _executor = value; }
        }

        /// <summary>
        /// ���������� � ������ ���������� ����������
        /// </summary>
        protected abstract void OnCommit();

        /// <summary>
        /// ���������� � ������ ������ ����������
        /// </summary>
        /// <param name="ex">����������</param>
        protected abstract void OnRollback(Exception ex);

        /// <summary>
        /// ���������� ������ � ��������� ����������
        /// </summary>
        public void Execute()
        {
            if (_executor == null)
                throw new ArgumentNullException("Executor");

            // ������� ����������
            using (IDbConnection connection = GetConnection())
            {
                // ������������� ���
                connection.Open();
                // �������� �����������
                using (IDbTransaction transaction = connection.BeginTransaction(
                    IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // ��������� ���������� �������
                        _executor(connection, transaction);
                        // ��������� ����������
                        transaction.Commit();
                        OnCommit();
                    }
                    catch (Exception ex)
                    {
                        // ����� ����������
                        SafeRollback(transaction);
                        OnRollback(ex);
                        // �������� ������� ����������
                        throw;
                    }
                }
            }
        }
    }
}
