using System;
using System.ComponentModel;

namespace ERPService.SharedLibs.PropertyGrid
{
    /// <summary>
    /// ���������, ����������� ���������� ����������� �������
    /// </summary>
    public interface IModalEditor
    {
        /// <summary>
        /// ����������� ���������� ���������
        /// </summary>
        /// <param name="descriptorContext">�������� ��� ��������� �������������� ���������� � ��������</param>
        bool ShowEditor(ITypeDescriptorContext descriptorContext);

        /// <summary>
        /// ������������� ��������
        /// </summary>
        Object Value { get; set; }
    }
}
