namespace ERPService.SharedLibs.Remoting
{
    /// <summary>
    /// ������� ����� ��� ��������, ��������������� ��� �������� �� ���������-������� c 
    /// "�����������" �������� �����
    /// </summary>
    public abstract class EternalHostingTarget : HostingTarget
    {
        /// <summary>
        /// ���������� ����� ����� ������� �� �������
        /// </summary>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
