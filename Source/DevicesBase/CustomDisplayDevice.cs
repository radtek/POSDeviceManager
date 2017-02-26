using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� �������� ����������
    /// </summary>
    public abstract class CustomDisplayDevice : CustomSerialDevice, ICustomerDisplay
	{
		#region �����������

		/// <summary>
		/// ������� ������� ����������
		/// </summary>
		protected CustomDisplayDevice() : base()
		{
		}

		#endregion

		#region ���������� ICustomerDisplay

		/// <summary>
		/// ������ ������� ���������� �� ����-������ �������
		/// </summary>
		public abstract void SaveToEEPROM();

		/// <summary>
		/// ����� ����� �� �������
		/// </summary>
		/// <param name="lineNumber">����� ������</param>
		public abstract string this[int lineNumber] { set; }

		#endregion
}
}
