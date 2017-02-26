namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ���������, ����������� ����� �����
    /// </summary>
    /// <typeparam name="T">���, � ������� �������� ����� (������, ������������ � �.�.)</typeparam>
    public interface IOptionsProvider<T>
    {
        /// <summary>
        /// ���������� ����� ����� � ��������� ���������
        /// </summary>
        EditableOption<T>[] Options { get; }
    }
}
