namespace DevicesCommon
{
    /// <summary>
    /// ��������� ��� �������� ����������
    /// </summary>
    public interface ICustomerDisplay : ISerialDevice
	{
        #region ��������

        /// <summary>
        /// ����� ����� �� �������
        /// </summary>
        /// <param name="lineNumber">����� ������</param>
        string this[int lineNumber] { set; }

#endregion

#region ������

		/// <summary>
		/// ������ ������� ���������� �� ����-������ �������
		/// </summary>
		void SaveToEEPROM();

#endregion
	}
}
