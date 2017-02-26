using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ERPService.SharedLibs.Helpers.Databases
{
    /// <summary>
    /// ������� ��������������� ����� ��� ������ � ����� ������
    /// </summary>
    public abstract class CustomDatabaseHelper : IDisposable
    {
        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomDatabaseHelper()
        {
        }

        /// <summary>
        /// ��������� �������� � �������
        /// </summary>
        /// <param name="command">�������</param>
        /// <param name="parameterName">��� ���������</param>
        /// <param name="parameterValue">�������� ���������</param>
        /// <returns>����� ����������� ��������</returns>
        public IDataParameter NewParameter(IDbCommand command, string parameterName,
            Object parameterValue)
        {
            // ������� ����� ��������
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;

            // ��������� ��� � ��������� ���������� �������
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// ���������� ����� ����������
        /// </summary>
        /// <param name="transaction">����������</param>
        public void SafeRollback(IDbTransaction transaction)
        {
            if (transaction == null)
                return;

            try
            {
                // ���������� ����������
                transaction.Rollback();
            }
            catch (InvalidOperationException)
            {
                // ���������� �������� ��� ������ ����������
            }
        }

        /// <summary>
        /// ������� ����������� � ���� ������
        /// </summary>
        /// <returns>����������� � ���� ������</returns>
        protected abstract IDbConnection GetConnection();

        #region ���������� IDisposable

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
