using System;
using System.Collections.Generic;
using System.Text;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� ��� �������-�������
    /// </summary>
    public class PasswordEditor : CustomModalEditor
    {
        /// <summary>
        /// ���������� ���������� ���������� ��� �������������� ��������
        /// </summary>
        protected override IModalEditor GetEditor()
        {
            return new FormPasswordEditor();
        }
    }
}
