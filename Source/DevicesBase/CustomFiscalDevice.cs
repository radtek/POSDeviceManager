using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
	/// <summary>
	/// ������� ����� ��� ���������� ���������
	/// </summary>
	public abstract class CustomFiscalDevice : CustomPrintableDevice, IFiscalDevice
	{
		// ������ ��
		private Int32 taxerPassword;

		#region �����������

		/// <summary>
		/// ������� ���������� ����������
		/// </summary>
		protected CustomFiscalDevice() : base()
		{
		}

		#endregion

		#region ���������� IFiscalDevice

		/// <summary>
		/// ���� � ����� ���������� ������
		/// </summary>
		public abstract DateTime CurrentTimestamp { get; set; }

		/// <summary>
		/// �������, ����������� ��� ������������� ������� ������������ 
		/// �� ��������� ����������� ����������
		/// </summary>
		public abstract event EventHandler<FiscalBreakEventArgs> FiscalBreak;

		/// <summary>
		/// ���������� �����
		/// </summary>
		/// <param name="reportType">��� ������</param>
		/// <param name="full">������ ��� ������� �����</param>
		/// <param name="reportParams">��������� ������ (������� �� ���� ������)</param>
		public abstract void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams);

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="newPassword">����� ������ ���������� ����������</param>
		/// <param name="registrationNumber">��������������� �����</param>
		/// <param name="taxPayerNumber">���</param>
		public abstract void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber);

		/// <summary>
		/// ��������� ��� � ����
		/// </summary>
		/// <param name="firstDate">���� ������ �������� �����</param>
		/// <param name="lastDate">���� ��������� �������� �����</param>
		/// <param name="firstShift">����� ������ �����</param>
		/// <param name="lastShift">����� ��������� �����</param>
		public abstract void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, 
			out int lastShift);

		/// <summary>
		/// ���������� �������������� ����������� ����������
		/// </summary>
		public abstract FiscalDeviceInfo Info { get; }

		/// <summary>
		/// ����� ���������
		/// </summary>
		public abstract FiscalStatusFlags Status { get; }

		/// <summary>
		/// ������� ������ ���������� ����������
		/// ������� ������ �� ���������� � �������������� ����� ������.
		/// �������� ������ ���� ���������������� �� ������ ������ ������� ������ ��
		/// </summary>
		public int TaxerPassword
		{
			get
			{
				return taxerPassword;
			}
			set
			{
				taxerPassword = value;
			}
		}

		#endregion
	}
}
