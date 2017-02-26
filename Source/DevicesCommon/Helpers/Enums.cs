namespace DevicesCommon.Helpers
{
    /// <summary>
    /// ����� ��� ���� ��������� ���� ������
    /// </summary>
    public enum GeneralError
	{
		/// <summary>
		/// ������� (������ ���)
		/// </summary>
		Success	= 0x1000,

		/// <summary>
		/// ���������� ������ ������ �����������
		/// </summary>
		Busy	= 0x1001,

		/// <summary>
		/// ������� �� ����� ������ ������� � �����������
		/// </summary>
		Timeout	= 0x1002,

        /// <summary>
        /// ���������� ���������. ��������� ������������ ���������� 
        /// ����� ���������� � ��� ������������ �������
        /// </summary>
        Inactive  = 0x1003,

        /// <summary>
        /// �������������� �������� �� ����� ������ ������� � �����������
        /// </summary>
        Exception = 0x1004,

        /// <summary>
        /// ������� �� �������������� �����������
        /// </summary>
        Unsupported = 0x1005,

        /// <summary>
        /// ������� �� �������������� ����������� ��� ������ ���������� �������
        /// </summary>
        CurrentlyUnsupported = 0x1006,

        /// <summary>
        /// ������, ������������� ��� ��������� ������ � �����������
        /// </summary>
        Specific = 0x1007
	}

	/// <summary>
	/// ��� ���������
	/// </summary>
	public enum DocumentType
	{
		/// <summary>
		/// �������
		/// </summary>
		Sale,

		/// <summary>
		/// �������
		/// </summary>
		Refund,

		/// <summary>
		/// �������� ����� � �����
		/// </summary>
		PayingIn,
		
		/// <summary>
		/// ������� ����� �� �����
		/// </summary>
		PayingOut,

        /// <summary>
        /// X-�����
        /// </summary>
        XReport,

        /// <summary>
        /// Z-�����
        /// </summary>
        ZReport,

        /// <summary>
        /// ����� �� �������
        /// </summary>
        SectionsReport,

		/// <summary>
		/// ������������ ��������
		/// </summary>
		Other
	}

    /// <summary>
    /// ����� �������� ��� ������ ���������
    /// </summary>
    public enum PrinterNumber
    {
        /// <summary>
        /// �������� ������� ����������
        /// </summary>
        MainPrinter,

        /// <summary>
        /// ������ �������������� ������� ����������
        /// </summary>
        AdditionalPrinter1,

        /// <summary>
        /// ������ �������������� ������� ����������
        /// </summary>
        AdditionalPrinter2
    }

	/// <summary>
	/// ������������ ������ ��� ������
	/// </summary>
	public enum AlignOptions
	{
		/// <summary>
		/// �� ������ ����
		/// </summary>
		Left,

		/// <summary>
		/// �� ������
		/// </summary>
		Center,

		/// <summary>
		/// �� ������� ����
		/// </summary>
		Right
	}

	/// <summary>
	/// ���� ������
	/// </summary>
	public enum FiscalPaymentType
	{
		/// <summary>
		/// ���������
		/// </summary>
		Cash,
		
		/// <summary>
		/// ��������� ������
		/// </summary>
		Card,
		
		/// <summary>
		/// ������������ ����������� ��� ������ �1
		/// </summary>
		Other1,
		
		/// <summary>
		/// ������������ ����������� ��� ������ �2
		/// </summary>
		Other2,
		
		/// <summary>
		/// ������������ ����������� ��� ������ �3
		/// </summary>
		Other3
	}

	/// <summary>
	/// ��� �������� ������
	/// </summary>
	public enum ShiftReportType
	{
		/// <summary>
		/// ����� ��� �������
		/// </summary>
		ReportX,

		/// <summary>
		/// ����� �� �������
		/// </summary>
		ReportSections,

		/// <summary>
		/// ����� � ��������
		/// </summary>
		ReportZ
	}

	/// <summary>
	/// ��� ����������� ������
	/// </summary>
	public enum FiscalReportType
	{
		/// <summary>
		/// �� �����
		/// </summary>
		ByDates,

		/// <summary>
		/// �� ������
		/// </summary>
		ByShifts
	}

	/// <summary>
	/// ������� ��������� ������� �� ��������
	/// </summary>
	public enum PrinterBreak
	{
		/// <summary>
		/// ����������� ������
		/// </summary>
		PaperOut,

		/// <summary>
		/// �������� �������� ����
		/// </summary>
		OpenedDrawer
	}

	/// <summary>
	/// ������� ��������� ������� �� ��
	/// </summary>
	public enum FiscalBreak
	{
		/// <summary>
		/// ������� ������� �� � �� ����� 10 �����
		/// </summary>
		TimeDeltaIsLarge,

		/// <summary>
		/// ����������� ������������
		/// </summary>
		Locked,

		/// <summary>
		/// ����� ��������� 24 ����
		/// </summary>
		OverShift,

		/// <summary>
		/// �������� �����
		/// </summary>
		OpenedShift,

		/// <summary>
		/// ������ ��������� ������ � ��
		/// </summary>
		ProtocolError,

		/// <summary>
		/// �������� ��������
		/// </summary>
		OpenedDocument
	}

	/// <summary>
	/// ������ ���������� ������ � �������� �����
	/// </summary>
	public enum PaperOutStatus
	{
		/// <summary>
		/// ������ ����
		/// </summary>
		Present,

		/// <summary>
		/// ������ ����������� (��������� ���������� ������)
		/// </summary>
		OutPassive,

		/// <summary>
		/// ������ ����������� (�������� ���������� ������, �����������
		/// ������� ����������� ������)
		/// </summary>
		OutActive,

		/// <summary>
		/// ������ ���� (����� ��������� ��������� ������, ��������� 
		/// ������� ����������� ������)
		/// </summary>
		OutAfterActive
	}

	/// <summary>
	/// ����� ������ �� ������� ���������
	/// </summary>
	public enum FontStyle
	{
		/// <summary>
		/// �������
		/// </summary>
		Regular,

		/// <summary>
		/// ������� ������
		/// </summary>
		DoubleHeight,

		/// <summary>
		/// ������� ������
		/// </summary>
		DoubleWidth,

		/// <summary>
		/// ������� ������ � ������� ������
		/// </summary>
		DoubleAll
	}

    /// <summary>
    /// ������� �������� (��� ���������, ����������� INetworkDevice)
    /// </summary>
    public enum NetworkProtocol
    {
        /// <summary>
        /// TCP-����������
        /// </summary>
        TCP,

        /// <summary>
        /// UDP-����������
        /// </summary>
        UDP
    }

    /// <summary>
    /// ��� ����������� ����������
    /// </summary>
    public enum PrinterKind
    {
        /// <summary>
        /// ��������
        /// </summary>
        Receipt,

        /// <summary>
        /// ����������
        /// </summary>
        Slip,

        /// <summary>
        /// ���������������
        /// </summary>
        Combo
    }
}