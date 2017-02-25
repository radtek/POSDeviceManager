using System;
using System.Collections.Generic;
using System.Text;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// Вспомогательный класс для работы с текстовыми протоколами через последовательный порт
    /// </summary>
    public sealed class TextProtocol
    {
        private EasyCommunicationPort _port;
        private Encoding _encoding;
        private Int32 _receiveTimeout;

        private Boolean IsStopByte(Byte b)
        {
            return (b == 10 || b == 13);
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="encoding">Кодировка для преобразования строк</param>
        /// <param name="port">Коммуникационный порт</param>
        public TextProtocol(Encoding encoding, EasyCommunicationPort port)
        {
            _encoding = encoding;
            _port = port;
            _port.WriteTimeout = 1000;
            _port.ReadTimeout = -1;
            _receiveTimeout = 5000;
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="port">Коммуникационный порт</param>
        public TextProtocol(EasyCommunicationPort port)
            : this(Encoding.Default, port)
        {
        }

        /// <summary>
        /// Таймаут получения ответа
        /// </summary>
        public Int32 ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set { _receiveTimeout = value; }
        }

        /// <summary>
        /// Отправка команды
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns>Ответ</returns>
        public String Send(String command)
        {
            // добавляем завершающий символ к команде
            String preparedCommand = String.Concat(command, "\r");

            // пишем команду в порт
            _port.ClearError();
            _port.DiscardBuffers();
            _port.Write(_encoding.GetBytes(preparedCommand));

            // засекаем время
            DateTime fixedTime = DateTime.Now;

            // буфер для хранения ответа
            List<Byte> answer = new List<Byte>();
            // очередной байт ответа
            Byte nextByte = 0;
            // временный буфер данных
            Byte[] buf = new Byte[1024];

            // читаем ответ
            do
            {
                // читаем очередную порцию данных и з порта
                Array.Clear(buf, 0, buf.Length);
                Int32 bytesRead = _port.Read(buf, 0, buf.Length);
                
                // разбираем полученные данные
                for (Int32 i = 0; i < bytesRead; i++)
                {
                    nextByte = buf[i];

                    if (IsStopByte(nextByte))
                        // стоповый байт
                        break;

                    if (nextByte >= 0x20)
                        // читаемый символ или его часть
                        answer.Add(nextByte);
                }

                if (IsStopByte(nextByte))
                    break;
                else
                {
                    TimeSpan elapsedTime = DateTime.Now - fixedTime;
                    if (elapsedTime.TotalMilliseconds >= _receiveTimeout)
                        throw new TimeoutException("Время ожидания ответа истекло");
                }
            }
            while (!IsStopByte(nextByte));

            // возвращаем ответ в виде строки в текущей кодировке
            return _encoding.GetString(answer.ToArray());
        }
    }
}
