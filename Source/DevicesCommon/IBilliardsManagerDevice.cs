namespace DevicesCommon
{
    /// <summary>
    /// ��������� ����������, ������������ ����������� �������
    /// </summary>
    public interface IBilliardsManagerDevice : ISerialDevice
    {
        /// <summary>
        /// �������� ���� ��� ���������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        void LightsOn(int billiardTableNo);

        /// <summary>
        /// ��������� ���� ��� ���������� ������
        /// </summary>
        /// <param name="billiardTableNo">����� ����������� �����</param>
        void LightsOff(int billiardTableNo);
    }
}
