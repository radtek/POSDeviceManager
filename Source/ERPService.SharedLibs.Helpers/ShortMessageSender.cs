using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// Отправитель коротких текстовых сообщений
    /// </summary>
    public class ShortMessageSender : IDisposable
    {
        #region Поля

        private EasyCommunicationPort _port;
        private string[] _finalResults;

        private const int _waitForCommand = 7000;
        private const string _opCancelled = 
            "Отправка короткого текстового сообщения прервана. Команда: {0}. Ошибка: {1}";
        private const string _invalidIndexes = "Не возможно определить индексы хранилища сообшений. Ответ: {0}";
        private const string _storageIndexesPattern =
            @"(?:\u002BCMGD\u003A\s\u0028)(\d+)(?:-)(\d+)(?:\u0029)";

        #endregion

        #region Закрытые методы

        /// <summary>
        /// Результат успешного выполнения большинства команд
        /// </summary>
        private string OkResult
        {
            get { return _finalResults[0]; }
        }

        /// <summary>
        /// Проверка завершения получения ответа от телефона
        /// </summary>
        /// <param name="answers">Список уже полученных ответов</param>
        private bool FinalResultReceived(List<string> answers)
        {
            // проверяем, получен ли один из финальных результатов
            foreach (string answer in answers)
            {
                foreach (string finalResult in _finalResults)
                {
                    if (answer.Contains(finalResult))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Запись очередного ответа, полученного от телефона, в список
        /// </summary>
        /// <param name="sb">Ответ</param>
        /// <param name="answers">Список ответов</param>
        private void SaveAnswer(StringBuilder sb, List<string> answers)
        {
            if (sb.Length > 0)
            {
                answers.Add(sb.ToString());
                sb.Length = 0;
            }
        }

        /// <summary>
        /// Чтение ответа на AT-команду
        /// </summary>
        /// <param name="answers">Список для хранения ответных строк</param>
        private void ReceiveATCommandResponse(List<string> answers)
        {
            // увеличиваем таймаут на чтение для того, чтобы дать время
            // телефону обработать команду и начать выдавать ответ на нее
            _port.ReadTimeout = _waitForCommand;

            // здесь хранится очередная формируемая строка ответа от телефона
            StringBuilder sb = new StringBuilder();

            // читаем до получения какого-либо из финальных результатов
            do
            {
                // очередной байт, полученный от телефона
                int nextByte;
                // читаем до первого таймаута
                do
                {
                    nextByte = _port.ReadByte();
                    if (nextByte > 0)
                    {
                        // что-то прочитали
                        // телефон начал передавать ответ на команду, поэтому
                        // возвращаем таймаут на место
                        if (_port.ReadTimeout == _waitForCommand)
                            _port.ReadTimeout = 0;

                        // проверим, не пришел ли символ-терминатор
                        if (nextByte == 0x0D)
                            // сохраняем очередную строку ответа
                            SaveAnswer(sb, answers);
                        else
                        {
                            if (nextByte >= 0x20)
                                // в ответ добавляем только читаемые сиволы
                                sb.Append((Char)nextByte);
                        }
                    }
                }
                while (nextByte > 0);

                // проверяем, не возник ли таймаут при чтении первого байта
                if (_port.ReadTimeout == _waitForCommand)
                    // телефон не ответил на команду, проблемы со связью
                    return;

                // помещаем остаток формируемой строки в список
                SaveAnswer(sb, answers);
            }
            while (!FinalResultReceived(answers));
        }

        /// <summary>
        /// Отправка AT-команды телефону
        /// </summary>
        /// <param name="command">Текст команды</param>
        /// <param name="terminator">Завершающий символ</param>
        /// <returns>Список строк, полученных в ответ на команду</returns>
        private string[] SendATCommand(string command, byte terminator)
        {
            // пишем команду и завершающий символ в порт
            _port.DiscardBuffers();
            _port.Write(Encoding.ASCII.GetBytes(command));
            _port.WriteByte(terminator);

            // отключаем генерацию исключений по таймауту (особенность AT-протокола)
            _port.ThrowTimeoutExceptions = false;            
            try
            {
                // читаем ответ модема
                List<string> answers = new List<string>();
                ReceiveATCommandResponse(answers);
                return answers.ToArray();
            }
            finally
            {
                // возвращаем флаг генерации исключений
                _port.ThrowTimeoutExceptions = true;
            }
        }

        /// <summary>
        /// Отправка AT-команды телефону. Команда заверщается символом CR
        /// </summary>
        /// <param name="command">Текст команды</param>
        private string[] SendATCommand(string command)
        {
            return SendATCommand(command, 0x0D);
        }

        /// <summary>
        /// Проверка статуса выполнения команды
        /// </summary>
        /// <param name="answers">Список ответов на команду</param>
        /// <param name="okValue">Значение, соответствующее успешному выполнению команды</param>
        private void CheckCommandStatus(string[] answers, string okValue)
        {
            if (answers.Length == 0)
                throw new InvalidOperationException("Неверный ответ на команду");

            if (IsFail(answers, okValue))
                throw new OperationCanceledException(
                    string.Format(_opCancelled, answers[0], answers[answers.Length - 1]));
        }

        /// <summary>
        /// Проверка статуса выполнения команды, для которой ожидается ответ "OK"
        /// </summary>
        /// <param name="answers">Список ответов на команду</param>
        private void CheckCommandStatus(string[] answers)
        {
            CheckCommandStatus(answers, OkResult);
        }

        /// <summary>
        /// Возвращает признак неуспешного выполнения команды
        /// </summary>
        /// <param name="answers">Список ответов на команду</param>
        /// <param name="okValue">Значение, соответствующее успешному выполнению команды</param>
        private bool IsFail(string[] answers, string okValue)
        {
            return !answers[answers.Length - 1].Contains(okValue);
        }

        /// <summary>
        /// Возвращает диапазон индексов хранилищ сообщений
        /// </summary>
        /// <param name="minValue">Минимальный индекс</param>
        /// <param name="maxValue">Максимальный индекс</param>
        private void GetStoragesRange(out int minValue, out int maxValue)
        {
            // запрашиваем диапазон индексов
            string[] answers = SendATCommand("AT+CMGD=?");
            CheckCommandStatus(answers);

            // разбираем полученный ответ
            foreach (string answer in answers)
            {
                Match match = Regex.Match(answer, _storageIndexesPattern);
                if (match.Success)
                {
                    minValue = Convert.ToInt32(match.Groups[1].Value);
                    maxValue = Convert.ToInt32(match.Groups[2].Value);
                    return;
                }
            }
            throw new InvalidOperationException(string.Format(_invalidIndexes, answers[0]));
        }

        /// <summary>
        /// Подготовка порта к работе
        /// </summary>
        private void PreparePort()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
                _port.WriteTimeout = 2000;
                // будет использоваться только таймаут приема байта
                _port.ReadTimeout = 0;
            }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="portName">Имя порта модема</param>
        /// <param name="baud">Скорость связи с модемом</param>
        public ShortMessageSender(string portName, int baud)
        {
            _port = new EasyCommunicationPort();
            _port.PortName = portName;
            _port.BaudRate = baud;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            _port.DsrFlow = false;

            _finalResults = new string[] { "OK", ">", "ERROR", "+CMS ERROR", "Call Ready" };
        }

        #endregion

        #region Открытые методы

        /// <summary>
        /// Очистка памяти телефона от сообщений
        /// </summary>
        public void CleanupMessages()
        {
            PreparePort();
            // определяем диапазон индексов хранилища сообщений
            int minIndex, maxIndex;
            GetStoragesRange(out minIndex, out maxIndex);

            for (int i = minIndex; i <= maxIndex; i++)
                CheckCommandStatus(SendATCommand(string.Format("AT+CMGD={0}", i)));
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message">Сообщение, подготовленное к отправке</param>
        public void Send(EncodedMessage message)
        {
            PreparePort();
            // включаем режим отправки SMS в PDU-формате
            CheckCommandStatus(SendATCommand("AT+CMGF=0"));
            // проверяем, поддреживает ли телефон SMS-команды
            CheckCommandStatus(SendATCommand("AT+CSMS=0"));
            // отправляем заголовок сообщения
            CheckCommandStatus(
                SendATCommand(string.Format("AT+CMGS={0}", message.NumberOfOctets)), ">");
            // отправляем тело сообщения
            CheckCommandStatus(SendATCommand(message.Message, 0x1A));
        }

        /// <summary>
        /// Отправка нескольких сообщений
        /// </summary>
        /// <param name="messages">Сообщения, подготовленные к отправке</param>
        public void Send(EncodedMessage[] messages)
        {
            Send(messages, true);
        }

        /// <summary>
        /// Отправка нескольких сообщений
        /// </summary>
        /// <param name="messages">Сообщения, подготовленные к отправке</param>
        /// <param name="reverseOrder">Отправить сообщения в обратном порядке</param>
        public void Send(EncodedMessage[] messages, bool reverseOrder)
        {
            if (reverseOrder)
            {
                for (int i = messages.Length; i > 0; i--)
                    Send(messages[i - 1]);
            }
            else
            {
                foreach (EncodedMessage message in messages)
                    Send(message);
            }
        }

        /// <summary>
        /// Перезагрузка устройства для отправки сообщений
        /// </summary>
        public virtual void RebootGSMDevice()
        {
            PreparePort();
            // отправляем команду выключения питания GPRS-модуля
            // для USB-модемов Teleofis RX101 эта команда выполняет перезагрузку
            CheckCommandStatus(SendATCommand("AT+CPOWD=1"), "Call Ready");
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (_port != null)
            {
                _port.Dispose();
                _port = null;
            }
        }

        #endregion
    }
}
