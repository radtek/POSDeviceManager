using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���� ������������ ���������
    /// </summary>
    public abstract class CustomConnectableDevice : CustomDevice, IConnectableDevice
	{
		#region �����������

		/// <summary>
		/// ������� ������������ ����������
		/// </summary>
		protected CustomConnectableDevice(): base()
		{
		}

		#endregion

		#region ���������� IConnectableDevice

		/// <summary>
		/// ��� ����� (����., COM1, LPT1...)
		/// </summary>
		public abstract string PortName { get; set; }

		#endregion
	}
}
