using System;
using System.Runtime.Serialization;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������������� ����� ��� ������ � <see cref="SerializationInfo"/>
    /// </summary>
    public sealed class SerializationInfoHelper
    {
        private SerializationInfo _info;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="info">���������� ��� ������������/��������������</param>
        public SerializationInfoHelper(SerializationInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            _info = info;
        }

        /// <summary>
        /// ������� �������� ��� ��������������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="name">������������ ��������</param>
        /// <returns>����������������� ��������</returns>
        public T GetValue<T>(string name)
        {
            return (T)_info.GetValue(name, typeof(T));
        }

        /// <summary>
        /// ���������� �������� ��� ������������
        /// </summary>
        /// <typeparam name="T">��� ��������</typeparam>
        /// <param name="name">������������ ��������</param>
        /// <param name="value">��������</param>
        public void AddValue<T>(string name, T value)
        {
            _info.AddValue(name, value, typeof(T));
        }
    }
}
