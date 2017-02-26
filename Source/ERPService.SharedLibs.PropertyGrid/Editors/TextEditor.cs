using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� ��� �������-��������� ������
    /// </summary>
    public sealed class TextEditor : CustomModalEditor
    {
        /// <summary>
        /// ���������� ������ �� ��������� ���������� ���������
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormTextEditor();
        }
    }
}
