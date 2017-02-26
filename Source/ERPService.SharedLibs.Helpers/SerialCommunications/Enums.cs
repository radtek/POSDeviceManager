namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum Parity : byte
    {
        /// <summary>
        /// ���
        /// </summary>
        None,

        /// <summary>
        /// ��������
        /// </summary>
        Odd,

        /// <summary>
        /// ������
        /// </summary>
        Even,

        /// <summary>
        /// �� ���������� ���� ��������
        /// </summary>
        Mark,

        /// <summary>
        /// �� �������� ���� ��������
        /// </summary>
        Space
    }

    /// <summary>
    /// �������� ���
    /// </summary>
    public enum StopBits : byte
    {
        /// <summary>
        /// ����
        /// </summary>
        One,

        /// <summary>
        /// �������
        /// </summary>
        OneAndHalf,

        /// <summary>
        /// ���
        /// </summary>
        Two
    }
}
