using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesCommon.Helpers
{
    /// <summary>
    /// Код ошибки, отправляемый устройством
    /// </summary>
    [Serializable]
    public class ErrorCode
    {
        #region Константы

        // шаблон полного описания кода ошибки
        private const String fullDescriptionTemplate = "Устройство: \"{0}\".\nОбщий код ошибки: {1} ({2}).\nДополнительный код ошибки: {3} ({4})";

        #endregion

        #region Поля

        // идентификатор устройства, которому принадлежит код ошибки
        private String _sender;
        // общий код ошибки
        private GeneralError _value;
        // описание общего кода ошибки
        private String _description;
        // специфический для протокола обмена с устройством код ошибки
        private Int16 _specificValue;
        // описание специфического для протокола обмена с устройством кода ошибки
        private String _specificDescription;

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
        public ErrorCode(String sender, GeneralError value, String description,
            Int16 specificValue, String specificDescription)
        {
            _sender = sender;
            _value = value;
            _description = description;
            _specificValue = specificValue;
            _specificDescription = specificDescription;
        }

        #endregion

        #region Методы, доступные из потомков

        /// <summary>
        /// Возвращает описание общего кода ошибки по его значению
        /// </summary>
        /// <param name="value">Значение кода ошибки</param>
        protected static String GetGeneralDescription(GeneralError value)
        {
            switch (value)
            {
                case GeneralError.Success:
                    return "Успешно (ошибок нет)";
                case GeneralError.Busy:
                    return "Устройство занято другим приложением";
                case GeneralError.Timeout:
                    return "Таймаут во время обмена данными с устройством";
                case GeneralError.Inactive:
                    return "Устройство неактивно. Требуется активировать устройство перед обращением к его интерфейсным методам";
                case GeneralError.Exception:
                    return "Исключительная ситуация во время обмена данными с устройством";
                case GeneralError.Unsupported:
                    return "Функция не поддерживается устройством";
                case GeneralError.CurrentlyUnsupported:
                    return "Функция не поддерживается устройством при данных параметрах команды";
                default:
                    return "Ошибка, специфическая для протокола обмена с устройством";
            }
        }

        #endregion

        #region Открыте свойства

        /// <summary>
        /// Идентификатор устройства, которому принадлежит код ошибки
        /// </summary>
        public String Sender
        {
            get { return _sender; }
        }

        /// <summary>
        /// Общий код ошибки
        /// </summary>
        public GeneralError Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Описание общего кода ошибки
        /// </summary>
        public String Description 
        {
            get { return _description; }
        }

        /// <summary>
        /// Специфический для протокола обмена с устройством код ошибки
        /// </summary>
        public short SpecificValue
        {
            get { return _specificValue; }
        }

        /// <summary>
        /// Описание специфического для протокола обмена с устройством кода ошибки
        /// </summary>
        public String SpecificDescription
        {
            get { return _specificDescription; }
        }

        /// <summary>
        /// Возвращает полное описание кода ошибки
        /// </summary>
        public string FullDescription
        {
            get
            {
                return String.Format(
                    fullDescriptionTemplate,
                    _sender,
                    _value,
                    _description,
                    _specificValue,
                    _specificDescription);
            }
        }

        /// <summary>
        /// Успешное завершение предыдущей операции
        /// </summary>
        public Boolean Succeeded
        {
            get { return _value == GeneralError.Success; }
        }

        /// <summary>
        /// Неуспешное завершение предыдущей операции
        /// </summary>
        public Boolean Failed
        {
            get { return _value != GeneralError.Success; }
        }

        #endregion

        #region Перегрузка методов базового класса

        /// <summary>
        /// Возвращает строковое представление объекта (краткое описание кода ошибки)
        /// </summary>
        public override String ToString()
        {
            switch (_value)
            {
                case GeneralError.Specific:
                    return _specificDescription;
                case GeneralError.Exception:
                    return _specificDescription;
                default:
                    return GetGeneralDescription(Value);
            }
        }

        #endregion
    }
}
