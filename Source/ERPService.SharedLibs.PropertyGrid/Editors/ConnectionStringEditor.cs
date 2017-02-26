using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� ��� �������, ���������� �������� ����������� � ���� ������
    /// </summary>
    public class ConnectionStringEditor : CustomModalEditor
    {
        /// <summary>
        /// ���������� ���������� ���������� ��� �������������� ��������
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormConnectionStringEditor();
        }
    }
}
