using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon.Helpers
{
	/// <summary>
	/// ��������������� �����, ������ ��������� ����� ���������
	/// </summary>
	public static class DeviceNames
	{
		#region ���������� ������������

		/// <summary>
		/// �� ������������ �����-�
		/// </summary>
		public const String ecrTypeStroke = "�����";

		/// <summary>
		/// �� ������������ ����� �����, ������ 1
		/// </summary>
        public const String ecrTypeSpark = "�����";

		/// <summary>
		/// �� ������������ ����-����������
		/// </summary>
        public const String ecrTypeAtol = "����";

		/// <summary>
		/// �� ������������ �����
		/// </summary>
        public const String ecrTypePilot = "�����";

		/// <summary>
		/// �� ������������ ��������
		/// </summary>
        public const String ecrTypeIncotex = "��������";

        /// <summary>
        /// �� ������������ ����� �����, ������ 2
        /// </summary>
        public const String ecrTypeSpark2 = "�����2";

        /// <summary>
        /// �� ������������ ������-����
        /// </summary>
        public const String ecrTypeServicePlus = "����������";

		#endregion

		#region �������� �����

		/// <summary>
		/// ����������� ������� ����� �� ��������� ESC POS
		/// </summary>
        public const String printerTypeGenericEpson = "Epson";

        /// <summary>
        /// ������� ���������� ���������� �� ��������� Star
        /// </summary>
        public const String printerTypeStarSlipPrinter = "Star";

		#endregion

		#region �����������

		/// <summary>
		/// RFID-����������� ������������ Iron Logic
		/// </summary>
        public const String ironLogicRFIDReader = "Iron Logic RFID";

		#endregion

		#region ������� ����������

		/// <summary>
		/// Firich VFD
		/// </summary>
        public const String customerDisplayVFD = "VFD";

		/// <summary>
		/// DSP
		/// </summary>
        public const String customerDisplayDSP = "DSP";

		/// <summary>
		/// Epson
		/// </summary>
        public const String customerDisplayEpson = "Epson";

        /// <summary>
        /// Aedex
        /// </summary>
        public const String customerDisplayAedex = "Aedex";

		#endregion

        #region ������ ���������� ���������

        /// <summary>
        /// ������ ���������� ��������� ����� ���������� CL-8RC
        /// </summary>
        public const String blcCl8rc = "���������� CL-8RC";

        #endregion

        #region ����

        /// <summary>
        /// ���� DIGI ����������
        /// </summary>
        public const String digiSimpleScales = "DIGI ����������";

        #endregion

        #region SMS-������

        /// <summary>
        /// ����������� GSM-����� (AT-�������)
        /// </summary>
        public const String standardGSMModem = "����������� GSM-����� (AT-�������)";

        #endregion

        #region ������ ���������� �����������

        /// <summary>
        /// ������ ���������� ���������� T283 ����� ���������� NL-16D0-DI3
        /// </summary>
        public const String t283dualTripod = "T283 (�������������)";

        #endregion
    }
}
