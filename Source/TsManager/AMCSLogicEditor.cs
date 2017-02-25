using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
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
