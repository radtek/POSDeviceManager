using System;
using System.Drawing;
using DevicesCommon.Helpers;

namespace DevicesCommon
{
    /// <summary>
    /// ��������� ����������� ������� ����������� ����������
    /// </summary>
    public class PrinterBreakEventArgs : EventArgs
	{
		private PrinterBreak breakReason;
		private bool canContinue;

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="breakReason">������� ������</param>
		public PrinterBreakEventArgs(PrinterBreak breakReason) : base()
		{
			this.breakReason = breakReason;
			canContinue = true;
		}

		/// <summary>
		/// ������� ������
		/// </summary>
		public PrinterBreak BreakReason
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
	/// ��������� �������� ����������� ����������
	/// </summary>
	public interface IPrintableDevice : ISerialDevice
	{
		#region �������� ����������� ����������

		/// <summary>
		/// ����� ��������� ��������
		/// </summary>
		PrinterStatusFlags PrinterStatus { get; }

		/// <summary>
		/// ���������� �������������� ����������� ����������
		/// </summary>
		PrintableDeviceInfo PrinterInfo { get; }

        /// <summary>
        /// �������� ��������� �����
        /// </summary>
        void OpenDrawer();


        #region ����� � ������ ���������

        /// <summary>
		/// ��������� ���������
		/// </summary>
        string[] DocumentHeader { get; set; }

        /// <summary>
        /// ������ ���������
        /// </summary>
        string[] DocumentFooter { get; set; }

        /// <summary>
        /// ����������� ��������� ���������
        /// </summary>
        Bitmap GraphicHeader { get; set; }

        /// <summary>
        /// ����������� ������ ���������
        /// </summary>
        Bitmap GraphicFooter { get; set; }

        /// <summary>
        /// �������� ����������� ��������� ����
        /// </summary>
        bool PrintGraphicHeader { get; set; }

        /// <summary>
        /// �������� ����������� ������ ���������
        /// </summary>
        bool PrintGraphicFooter { get; set; }

        /// <summary>
        /// �������� ��������� ���������
        /// </summary>
        bool PrintHeader { get; set; }

        /// <summary>
        /// �������� ������ ���������
        /// </summary>
        bool PrintFooter { get; set; }

        #endregion

        /// <summary>
		/// ������-����������� ���������� ������ ���������
		/// </summary>
		Char Separator { get; set; }

		#endregion

		#region ������ � ����������

		/// <summary>
		/// ������ ���������
		/// </summary>
		/// <param name="xmlData">������ XML-���������</param>
		void Print(string xmlData);

		#endregion

		#region �������

		/// <summary>
		/// �������, ����������� ��� ������������� ������� ������������
		/// �� ��������� ����������� ����������
		/// </summary>
		event EventHandler<PrinterBreakEventArgs> PrinterBreak;

		#endregion
	}
}
