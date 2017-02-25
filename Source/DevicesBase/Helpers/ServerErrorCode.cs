using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase.Helpers
{
    /// <summary>
    /// Серверный код ошибки
    /// </summary>
    [Serializable]
    public class ServerErrorCode : ErrorCode
    {
        #region Константы

        // шаблон сообщения для успешного выполнения действия
        private const String noErrors = "Ошибок нет";
        // шаблон описания ошибки, специфической для протокола обмена с устройством
        private const String specificError = "Ошибка {0}. {1}";
        // шаблон для хранения информации об исключении
        private const String exceptionText = "Исключение {0}. Текст: \"{1}\". Стек: \"{2}\"";

        #endregion

        #region Поля

        private String _commandDump;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр класса
        /// </summary>
        /// <param name="value">Общий код ошибки</param>
        /// <param name="description">Описание общего кода ошибки</param>
        /// <param name="specificValue">Код ошибки протокола обмена с устройством</param>
        /// <param name="specificDescription">Описание кода ошибки</param>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        /// <param name="commandDump"></param>
        public ServerErrorCode(IDevice sender, GeneralError value, String description,
            Int16 specificValue, String specificDescription, String commandDump)
            : base(sender.DeviceId, value, description, specificValue, specificDescription)
        {
            _commandDump = commandDump;
            if (Value == GeneralError.Success)
                return;

            // сохраняем ошибку в логе
            EventLogEntryType entryType = EventLogEntryType.Information;
            switch (Value)
            {
                // грубые ошибки
                case GeneralError.Busy:
                case GeneralError.Exception:
                case GeneralError.Specific:
                case GeneralError.Timeout:
                case GeneralError.Inactive:
                    entryType = EventLogEntryType.Error;
                    break;

                // предупреждения
                case GeneralError.CurrentlyUnsupported:
                case GeneralError.Unsupported:
                    entryType = EventLogEntryType.Warning;
                    break;
            }

            StringBuilder sb = new StringBuilder(FullDescription);
            sb.AppendLine("Дамп команды или ответа:");
            sb.AppendLine(String.IsNullOrEmpty(_commandDump) ? "Нет данных" : _commandDump);
            sender.Logger.WriteEntry(sb.ToString(), entryType);
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки из диапазона 
        /// общих кодов ошибок
        /// </summary>
        /// <param name="value">Общий код ошибки</param>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        public ServerErrorCode(IDevice sender, GeneralError value)
            : this(sender, value, GetGeneralDescription(value), 0, noErrors, String.Empty)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки из диапазона 
        /// общих кодов ошибок, включая дамп команды или ответа
        /// </summary>
        /// <param name="value">Общий код ошибки</param>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        /// <param name="commandDump">Дамп команды или ответа</param>
        public ServerErrorCode(IDevice sender, GeneralError value, String commandDump)
            : this(sender, value, GetGeneralDescription(value), 0, noErrors, commandDump)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки,
        /// специфической для протокола обмена с устройством
        /// </summary>
        /// <param name="specificValue">Код ошибки протокола обмена с устройством</param>
        /// <param name="specificDescription">Описание кода ошибки</param>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        public ServerErrorCode(IDevice sender, Int16 specificValue, String specificDescription)
            : this(sender, GeneralError.Specific, GetGeneralDescription(GeneralError.Specific),
            specificValue, specificDescription, String.Empty)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки,
        /// специфической для протокола обмена с устройством, включая дамп команды
        /// </summary>
        /// <param name="specificValue">Код ошибки протокола обмена с устройством</param>
        /// <param name="specificDescription">Описание кода ошибки</param>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        /// <param name="commandDump">Дамп команды или ответа</param>
        public ServerErrorCode(IDevice sender, Int16 specificValue, String specificDescription,
            String commandDump)
            : this(sender, GeneralError.Specific, GetGeneralDescription(GeneralError.Specific),
            specificValue, specificDescription, commandDump)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки, если тип общей ошибки - исключение
        /// </summary>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        /// <param name="ex">Исключение</param>
        public ServerErrorCode(IDevice sender, Exception ex)
            : this(sender, GeneralError.Exception,
            String.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0, 
            ex.Message, String.Empty)
        {
        }

        /// <summary>
        /// Создает экземпляр класса для хранения ошибки, если тип общей ошибки - исключение
        /// </summary>
        /// <param name="sender">Устройство-отправитель кода ошибки</param>
        /// <param name="ex">Исключение</param>
        /// <param name="commandDump">Дамп команды или ответа</param>
        public ServerErrorCode(IDevice sender, Exception ex, String commandDump)
            : this(sender, GeneralError.Exception,
            String.Format(exceptionText, ex.GetType().Name, ex.Message, ex.StackTrace), 0,
            ex.Message, commandDump)
        {
        }

        #endregion

        #region Открытые свойства

        /// <summary>
        /// Дамп команды или ответа
        /// </summary>
        public String CommandDump
        {
            get { return _commandDump; }
        }

        #endregion
    }
}
