using System;
using System.Net.Sockets;
using DevicesCommon.Helpers;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Коммуникатор для TCP-соединения
    /// </summary>
    public sealed class TcpCommunicator : CustomCommunicator
    {
        Socket socket;
        string host;
        int port;

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="host">Имя хоста</param>
        /// <param name="port">Порт</param>
        public TcpCommunicator(string host, int port) 
            : base()
        {
            this.host = host;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.IP);
        }

        /// <summary>
        /// Таймаут чтения
        /// </summary>
        public override int ReadTimeout
        {
            get { return socket.ReceiveTimeout; }
            set { socket.ReceiveTimeout = value; }
        }

        /// <summary>
        /// Таймаут записи
        /// </summary>
        public override int WriteTimeout
        {
            get { return socket.SendTimeout; }
            set { socket.SendTimeout = value; }
        }

        /// <summary>
        /// Установка соединения с устройством
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
                    string.Format("Ошибка подключения к хосту {0}:{1}", host, port), e);
            }
        }

        /// <summary>
        /// Закрывает подключение к устройству
        /// </summary>
        public override void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// Чтение данных из коммуникатора
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер принимаемых данных</param>
        public override int Read(byte[] buffer, int offset, int size)
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
                        throw new CommunicationException(string.Format("Ошибка чтения из сокета"), e);
                }
            }
            while(received < size);

            return received;
        }

        /// <summary>
        /// Запись данных в коммуникатор
        /// </summary>
        /// <param name="buffer">Буфер данных</param>
        /// <param name="offset">Смещение от начала буфера</param>
        /// <param name="size">Размер записываемых данных</param>
        public override int Write(byte[] buffer, int offset, int size)
        {
            try
            {
                return socket.Send(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException e)
            {
                throw new CommunicationException(
                    string.Format("Ошибка записи в сокет"), e);
            }
        }
    }
}
