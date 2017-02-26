using ERPService.SharedLibs.PropertyGrid;

namespace TsManager
{
    /// <summary>
    /// �������� �������� ���������� ������ ������ ����
    /// </summary>
    public class AMCSLogicEditor : CustomModalEditor
    {
        /// <summary>
        /// ������ �� ��������� ���������
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormAMCSLogicEditor();
        }
    }
}
