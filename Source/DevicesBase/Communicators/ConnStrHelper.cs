using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DevicesBase.Communicators
{
    /// <summary>
    /// Вспомогательный класс для разбора элементов строки подключения
    /// </summary>
    public sealed class ConnStrHelper
    {
        private Match _connStrMatch;

        private void ThrowArgException(String connStr)
        {
            throw new ArgumentException(
                String.Format("Неверный формат стоки подключения: {0}", connStr));
        }

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="connStr">Строка подключения</param>
        public ConnStrHelper(String connStr)
        {
            if (String.IsNullOrEmpty(connStr))
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
        public String this[Int32 itemNo]
        {
            get
            {
                return _connStrMatch.Groups[itemNo].Value;
            }
        }
    }
}
