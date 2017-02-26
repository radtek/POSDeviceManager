using System;
using System.Reflection;
using System.Windows.Forms;
using ERPService.SharedLibs.PropertyGrid.Forms;

namespace TsManager
{
    /// <summary>
    /// ����� ��������� �������� ���������� ������ ������ ����
    /// </summary>
    public partial class FormAMCSLogicEditor : FormModalEditor
    {
        private Object _obj;
        private bool _modified;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public FormAMCSLogicEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ������������� ��������
        /// </summary>
        public override Object Value
        {
            get
            {
                return _modified ? _obj : propertyGrid1.SelectedObject;
            }
            set
            {
                // �������� ����� ������� (�������� �������� ��������� �������)
                Type objType = value.GetType();
                // ����� ��������� �������
                _obj = Activator.CreateInstance(objType);
                // �������� �������� ��������� �������
                foreach (PropertyInfo pInfo in objType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        Object propertyValue = pInfo.GetValue(value, null);
                        pInfo.SetValue(_obj, propertyValue, null);
                    }
                    catch (ArgumentException)
                    {
                        // �������� �� ������� ������� ��� ��������
                    }
                }
                propertyGrid1.SelectedObject = _obj;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // ���� ������� ���������
            _modified = true;
        }
    }
}