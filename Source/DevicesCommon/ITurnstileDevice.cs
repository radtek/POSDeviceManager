namespace DevicesCommon
{
    /// <summary>
    /// �����������, � ������� �������� ��������
    /// </summary>
    public enum TurnstileDirection
    {
        /// <summary>
        /// �� ����
        /// </summary>
        Entry,

        /// <summary>
        /// �� �����
        /// </summary>
        Exit
    }

    /// <summary>
    /// ��������� ��������� ��� ���������� ����������
    /// </summary>
    public interface ITurnstileDevice : IRS485Device
    {
        #region ���������� ���������� ���������

        /// <summary>
        /// �����������, � ������� �������� ��������
        /// </summary>
        TurnstileDirection Direction { get; set; }

        /// <summary>
        /// ������� �������� ���������
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <returns>true, ���� � ������� �������� ����� �������� �������� ������</returns>
        bool Open();

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="accessDenied">������ ��������</param>
        void Close(bool accessDenied);

        #endregion

        #region ������ � ����������������� �����������

        /// <summary>
        /// ��������� ���� ����������������� ������ �� ����������
        /// </summary>
        string IdentificationData { get; }

        #endregion
    }
}
