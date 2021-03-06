using System;
using System.ComponentModel;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// ������������ ��� ����������������� �����
    /// </summary>
    public sealed class SerialCommunicator : CustomCommunicator
    {
        // ���������������� ���� ��� ����� � �����������
        EasyCommunicationPort _port;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="portName">��� �����</param>
        /// <param name="baudRate">�������� ������</param>
        public SerialCommunicator(string portName, int baudRate)
            : base()
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _port.BaudRate = baudRate;
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        public override int ReadTimeout
        {
            get { return _port.ReadTimeout; }
            set { _port.ReadTimeout = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        public override int WriteTimeout
        {
            get { return _port.WriteTimeout; }
            set { _port.WriteTimeout = value; }
        }

        /// <summary>
        /// ��������� ���������� � �����������
        /// </summary>
        public override void Open()
        {
            try
            {
                _port.Open();
                _port.DiscardBuffers();
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("�� ������� ������� ���� {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// ��������� ����������� � ����������
        /// </summary>
        public override void Close()
        {
            try
            {
                _port.Close();
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("�� ������� ������� ���� {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// ������ ������ �� �������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ����������� ������</param>
        public override int Read(byte[] buffer, int offset, int size)
        {
            try
            {
                return _port.Read(buffer, offset, size);
            }
            catch (TimeoutException e)
            {
                throw new CommunicationException("������� ������ ������", e);
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("������ ������ �� ����� {0}", _port.PortName), e);
            }
        }

        /// <summary>
        /// ������ ������ � ������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ������������ ������</param>
        public override int Write(byte[] buffer, int offset, int size)
        {
            try
            {
                return _port.Write(buffer, offset, size);
            }
            catch (TimeoutException e)
            {
                throw new CommunicationException("������� ������ ������", e);
            }
            catch (Win32Exception e)
            {
                throw new CommunicationException(
                    string.Format("������ ������ �� ����� {0}", _port.PortName), e);
            }
        }
    }
}
