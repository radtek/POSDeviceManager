using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
	/// <summary>
	/// ��������� �������� ��� ����������� ������������
	/// </summary>
	public class FiscalBreakEventArgs : EventArgs
	{
		private FiscalBreak breakReason;
		private bool canContinue;

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="breakReason">������� ������</param>
		public FiscalBreakEventArgs(FiscalBreak breakReason) : base()
		{
			this.breakReason = breakReason;
			canContinue = true;
		}

		/// <summary>
		/// ������� ������
		/// </summary>
		public FiscalBreak BreakReason
		{
			get
			{
				return breakReason;
			}
		}

		/// <summary>
		/// ���� ����������� ������
		/// </summary>
		public bool CanContinue
		{
			get
			{
				return canContinue;
			}
			set
			{
				canContinue = value;
			}
		}
	}

	/// <summary>
	/// ��������� �������� ����������� ������������
	/// </summary>
	public interface IFiscalDevice : IPrintableDevice
	{
		#region �������� ����������� ������������

		/// <summary>
		/// ����� ���������
		/// </summary>
		FiscalStatusFlags Status { get; }

		/// <summary>
		/// ���������� �������������� ����������� ����������
		/// </summary>
		FiscalDeviceInfo Info { get; }

		/// <summary>
		/// ���� � ����� ���������� ������
		/// </summary>
		DateTime CurrentTimestamp { get; set; }

		#endregion

		#region �������

		/// <summary>
		/// �������, ����������� ��� ������������� ������� ������������ 
		/// �� ��������� ����������� ����������
		/// </summary>
		event EventHandler<FiscalBreakEventArgs> FiscalBreak;

		#endregion

		#region ������� � �������� ������ ���������� ����������

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="newPassword">����� ������ ���������� ����������</param>
		/// <param name="registrationNumber">��������������� �����</param>
		/// <param name="taxpayerNumber">���</param>
		void Fiscalization(Int32 newPassword, Int64 registrationNumber, Int64 taxpayerNumber);

		/// <summary>
		/// ���������� �����
		/// </summary>
		/// <param name="reportType">��� ������</param>
		/// <param name="full">������ ��� ������� �����</param>
		/// <param name="reportOptions">��������� ������ (������� �� ���� ������)</param>
		void FiscalReport(FiscalReportType reportType, bool full, params Object[] reportOptions);

		/// <summary>
		/// ��������� ��� � ����
		/// </summary>
		/// <param name="firstDate">���� ������ �������� �����</param>
		/// <param name="lastDate">���� ��������� �������� �����</param>
		/// <param name="firstShift">����� ������ �����</param>
		/// <param name="lastShift">����� ��������� �����</param>
		void GetLifetime(out DateTime firstDate, out DateTime lastDate,
			out Int32 firstShift, out Int32 lastShift);

		/// <summary>
		/// ������� ������ ���������� ����������
		/// ������� ������ �� ���������� � �������������� ����� ������.
		/// �������� ������ ���� ���������������� �� ������ ������ ������� ������ ��
		/// </summary>
		Int32 TaxerPassword { get; set;	}

		#endregion
	}
}
