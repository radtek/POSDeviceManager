using System;
using System.Text.RegularExpressions;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Вспомогательный класс для разбора элементов строки подключения
    /// </summary>
    public sealed class ConnStrHelper
    {
        private Match _connStrMatch;

        private void ThrowArgException(string connStr)
        {
            throw new ArgumentException(
                string.Format("Неверный формат стоки подключения: {0}", connStr));
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="connStr">Строка подключения</param>
        public ConnStrHelper(string connStr)
        {
            if (string.IsNullOrEmpty(connStr))
                ThrowArgException(connStr);

            Regex cstr = new Regex(@"(?<Protocol>\w+):\/\/(?<Host>[\w.]+\/?):(?<Port>\d+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            _connStrMatch = cstr.Match(connStr);
            if (!_connStrMatch.Success)
                ThrowArgException(connStr);
        }

        /// <summary>
        /// Возвращает элемент строки подключения по номеру
        /// </summary>
        /// <param name="itemNo">Номер элемента строки подключения</param>
        public string this[int itemNo]
        {
            get
            {
                return _connStrMatch.Groups[itemNo].Value;
            }
        }
    }
}
