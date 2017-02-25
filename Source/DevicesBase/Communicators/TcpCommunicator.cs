using System;
using System.Net.Sockets;
using DevicesCommon.Helpers;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// ������������ ��� TCP-����������
    /// </summary>
    public sealed class TcpCommunicator : CustomCommunicator
    {
        Socket socket;
        String host;
        Int32 port;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="host">��� �����</param>
        /// <param name="port">����</param>
        public TcpCommunicator(String host, Int32 port) 
            : base()
        {
            this.host = host;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.IP);
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        public override Int32 ReadTimeout
        {
            get { return socket.ReceiveTimeout; }
            set { socket.ReceiveTimeout = value; }
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        public override Int32 WriteTimeout
        {
            get { return socket.SendTimeout; }
            set { socket.SendTimeout = value; }
        }

        /// <summary>
        /// ��������� ���������� � �����������
        /// </summary>
        public override void Open()
        {
            try
            {
                socket.Connect(host, port);
            }
            catch (SocketException e)
            {
                throw new CommunicationException(
                    String.Format("������ ����������� � ����� {0}:{1}", host, port), e);
            }
        }

        /// <summary>
        /// ��������� ����������� � ����������
        /// </summary>
        public override void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// ������ ������ �� �������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ����������� ������</param>
        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
        {
            int received = 0;
            do
            {
                try
                {
                    if (size > 0)
                        received += socket.Receive(buffer, offset, size, SocketFlags.None);
                    else if (socket.Available > 0)
                        received += socket.Receive(buffer, 0, socket.Available, SocketFlags.None);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.WouldBlock ||
                        e.SocketErrorCode == SocketError.IOPending ||
                        e.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        System.Threading.Thread.Sleep(30);
                    else
                        throw new CommunicationException(String.Format("������ ������ �� ������"), e);
                }
            }
            while(received < size);

            return received;
        }

        /// <summary>
        /// ������ ������ � ������������
        /// </summary>
        /// <param name="buffer">����� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="size">������ ������������ ������</param>
        public override Int32 Write(Byte[] buffer, Int32 offset, Int32 size)
        {
            try
            {
                return socket.Send(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException e)
            {
                throw new CommunicationException(
                    String.Format("������ ������ � �����"), e);
            }
        }
    }
}
