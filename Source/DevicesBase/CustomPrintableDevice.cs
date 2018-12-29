using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
    #region Вспомогательные классы и перечисления

    /// <summary>
    /// Опция работы с денежным ящиком
    /// </summary>
    internal enum DrawerOption
    {
        /// <summary>
        /// Открыть до начала печати
        /// </summary>
        OpenBefore,

        /// <summary>
        /// Открыть после завершения печати
        /// </summary>
        OpenAfter,

        /// <summary>
        /// Не открывать
        /// </summary>
        OpenNever
    }

    /// <summary>
    /// Атрибуты столбца таблицы
    /// </summary>
    internal class ColumnAttributes
    {
        public AlignOptions align;
        public int width;
    }

    /// <summary>
    /// Параметры печати сетки таблицы
    /// </summary>
    [Flags]
    internal enum GridOption
    {
        /// <summary>
        /// Сетки нет
        /// </summary>
        None = 0,

        /// <summary>
        /// Верхняя граница шапки
        /// </summary>
        Top = 1,

        /// <summary>
        /// Граница между шапкой и телом таблицы
        /// </summary>
        Middle = 2,

        /// <summary>
        /// Нижняя граница тела таблицы
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// Все линии сетки
        /// </summary>
        All = 16
    }

    #endregion

    /// <summary>
    /// Базовый класс для печатающих устройств
    /// </summary>
    public abstract class CustomPrintableDevice : CustomSerialDevice, IPrintableDevice
    {
        // заголовки и подвалы
        private List<string> documentFooter;
        private List<string> documentHeader;
        private System.Drawing.Bitmap _graphicHeader;
        private System.Drawing.Bitmap _graphicFooter;

        // флаги печати заголовков и подвалов
        private bool _printHeader;
        private bool _printFooter;
        private bool _printGraphicHeader;
        private bool _printGraphicFooter;

        // символ-разделитель
        private Char separator;
        // предыдущий принятый к печати документ
        private string previousXmlData;
        // используется для функции прогона строки
        private const string LineFeedString = " ";
        // есть ли в документе строки типа "registration" с ненулевой суммой
        private bool hasNonzeroRegistrations;
        // номер принтера для документа
        private PrinterNumber printerNumber;

        #region Перегрузка методов базового класса

        /// <summary>
        /// После активации устройства
        /// </summary>
        protected override void OnAfterActivate()
        {
            // установка аппрартного управления потоком
            // по линиям DTR/DSR
            Port.DsrFlow = PrinterInfo.DsrFlowControl;
            base.OnAfterActivate();
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Создает печатающее устройство
        /// </summary>
        protected CustomPrintableDevice() : base()
        {
            separator = '=';
            previousXmlData = string.Empty;
            hasNonzeroRegistrations = false;
            printerNumber = PrinterNumber.MainPrinter;
            documentFooter = new List<string>();
            documentHeader = new List<string>();
        }

        #endregion

        #region Методы, реализуемые в потомках

        /// <summary>
        /// Вызывается после пропуска отступа от верхнего края листа 
        /// при заданном значении верхнего отступа
        /// </summary>
        protected virtual void OnCustomTopMarginPrinted()
        {
        }

        /// <summary>
        /// Вызывается при необходимости послать команду продолжения печати
        /// </summary>
        protected virtual void OnContinuePrint()
        {
        }

        /// <summary>
        /// Вызывается при открытии документа.
        /// </summary>
        /// <param name="docType">
        /// Тип документа.
        /// </param>
        /// <param name="cashierName">
        /// Имя кассира, открывающего документ.
        /// </param>
        /// <param name="cashierInn">
        /// ИНН кассира, открывающего документ.
        /// </param>
        /// <param name="customerPhoneOrEmail">
        /// Номер телефона или e-mail покупателя.
        /// </param>
        /// <remarks>
        /// Метод для поддержки 54-ФЗ.
        /// </remarks>
        protected virtual void OnOpenDocument(DocumentType docType, string cashierName, string cashierInn, string customerPhoneOrEmail)
        {
        }

        /// <summary>
        /// Вызывается при закрытии документа.
        /// </summary>
        /// <param name="cutPaper">
        /// <see langword="true"/>, если нужно выполнить отрезку чековой ленты;
        /// <see langword="false"/> в противном случае.
        /// </param>
        protected virtual void OnCloseDocument(bool cutPaper)
        {
        }

        /// <summary>
        /// Вызывается при открытии денежного ящика
        /// </summary>
        protected virtual void OnOpenDrawer()
        {
        }

        /// <summary>
        /// Вызывается при печати строки
        /// </summary>
        /// <param name="source">Строка для печати</param>
        /// <param name="style">Стиль текста строки</param>
        protected virtual void OnPrintString(string source, FontStyle style)
        {
        }

        /// <summary>
        /// Вызывается при печати штрих-кода
        /// </summary>
        /// <param name="align">Выравнивание штрихкода</param>
        /// <param name="barcode">Данные штрихкода</param>
        /// <param name="readable">Печатать или не печатать текст</param>
        protected virtual void OnPrintBarcode(string barcode, AlignOptions align, 
            bool readable)
        {

        }

        /// <summary>
        /// Вызывается при печати рисунка
        /// </summary>
        /// <param name="align">Выравнивание рисунка</param>
        /// <param name="image">Рисунок</param>
        protected virtual void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
        }

        /// <summary>
        /// Вызывается при регистрации позиции в кассовом документе.
        /// </summary>
        /// <param name="positionName">
        /// Наименование позиции.
        /// </param>
        /// <param name="quantity">
        /// Количество позиции, граммы.
        /// </param>
        /// <param name="price">
        /// Цена позиции, копейки.
        /// </param>
        /// <param name="amount">
        /// Сумма позиции, копейки.
        /// </param>
        /// <param name="section">
        /// Секция для регистрации.
        /// </param>
        /// <param name="vatRateId">
        /// Идентифкатор ставки НДС.
        /// </param>
        /// <remarks>
        /// Метод для поддержки 54-ФЗ и ФФД 1.0.
        /// </remarks>
        protected virtual void OnRegistration(string positionName, uint quantity, uint price, uint amount, byte section, byte vatRateId)
        {
        }

        /// <summary>
        /// Вызывается при регистрации позиции в кассовом документе.
        /// </summary>
        /// <param name="positionName">
        /// Наименование позиции.
        /// </param>
        /// <param name="quantity">
        /// Количество позиции, граммы.
        /// </param>
        /// <param name="price">
        /// Цена позиции, копейки.
        /// </param>
        /// <param name="amount">
        /// Сумма позиции, копейки.
        /// </param>
        /// <param name="section">
        /// Секция для регистрации.
        /// </param>
        /// <param name="vatRateId">
        /// Идентифкатор ставки НДС.
        /// </param>
        /// <param name="fiscalItemType">
        /// Признак предмета расчета.
        /// </param>
        /// <remarks>
        /// Метод для поддержки 54-ФЗ и ФФД 1.05.
        /// </remarks>
        protected virtual void OnRegistration(string positionName, uint quantity, uint price, uint amount, byte section, byte vatRateId, byte fiscalItemType)
        {
        }

        /// <summary>
        /// Вызывается при регистрации позиции в кассовом документе.
        /// </summary>
        /// <param name="commentary">
        /// Наименование позиции.
        /// </param>
        /// <param name="quantity">
        /// Количество позиции, граммы.
        /// </param>
        /// <param name="amount">
        /// Сумма позиции, копейки.
        /// </param>
        /// <param name="section">
        /// Секция для регистрации.
        /// </param>
        protected virtual void OnRegistration(string commentary, uint quantity, uint amount, byte section)
        {
        }

        /// <summary>
        /// Вызывается перед приемом первого платежа по чеку.
        /// </summary>
        /// <param name="registrationsAmount">
        /// Сумма регистраций в чеке.
        /// </param>
        /// <remarks>
        /// Метод для поддержки 54-ФЗ.
        /// Нужен для борьбы с округлением чека.
        /// </remarks>
        protected virtual void OnTrimDocumentAmount(uint registrationsAmount)
        {
        }

        /// <summary>
        /// Вызывается при оплате документа
        /// </summary>
        /// <param name="amount">Сумма оплаты</param>
        /// <param name="paymentType">Тип оплаты</param>
        protected virtual void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
        }

        /// <summary>
        /// Вызывается при указании суммы внесения или выплаты
        /// </summary>
        /// <param name="amount">Сумма внесения или выплаты</param>
        protected virtual void OnCash(uint amount)
        {
        }

        #endregion

        #region Свойства и методы, доступные из потомков

        /// <summary>
        /// Номер принтера для печати текущего документа
        /// </summary>
        protected PrinterNumber PrinterNumber
        {
            get
            {
                return printerNumber;
            }
        }

        /// <summary>
        /// Печать строк
        /// </summary>
        /// <param name="lines">Строки</param>
        /// <param name="style">Стиль строк</param>
        protected void PrintStrings(string[] lines, FontStyle style)
        {
            if (lines == null || lines.Length == 0)
                return;

            foreach (string line in lines)
            {
                OnPrintString(line, style);
                if (ErrorCode.Failed)
                    break;
            }
                
        }

        /// <summary>
        /// Печать заголовка документа
        /// </summary>
        /// <remarks>
        /// Для фискальных регистраторов нужно выполнять программирование
        /// заголовка чека в методе OnAfterActivate. Доступ к содержимому клише
        /// производить через свойства IPrintableDevice
        /// </remarks>
        protected void DrawHeader()
        {
            if (_printHeader && documentHeader.Count > 0)
            {
                PrintStrings(documentHeader.ToArray(), FontStyle.Regular);
            }
        }

        /// <summary>
        /// Печать подвала документа
        /// </summary>
        /// <remarks>
        /// Для фискальных регистраторов нужно выполнять программирование
        /// подвала чека в методе OnAfterActivate. Доступ к содержимому клише
        /// производить через свойства IPrintableDevice
        /// </remarks>
        protected void DrawFooter()
        {
            if (_printFooter && documentFooter.Count > 0)
            {
                PrintStrings(documentFooter.ToArray(), FontStyle.Regular);
            }
        }

        /// <summary>
        /// Печать графического заголовка
        /// </summary>
        protected void DrawGraphicHeader()
        {
            if (_printGraphicHeader && _graphicHeader != null)
            {
                OnPrintImage(_graphicHeader, AlignOptions.Center);
            }
        }

        /// <summary>
        /// Печать графического подвала
        /// </summary>
        protected void DrawGraphicFooter()
        {
            if (_printGraphicFooter && _graphicFooter != null)
            {
                OnPrintImage(_graphicFooter, AlignOptions.Center);
            }
        }

        /// <summary>
        /// Есть ли в документе строки типа "registration" с ненулевой суммой.
        /// Свойство определено с момента вызова OnOpenDocument и до вызова OnCloseDocument
        /// </summary>
        protected bool HasNonzeroRegistrations
        {
            get { return hasNonzeroRegistrations; }
        }

        /// <summary>
        /// Печать верхнего отступа
        /// </summary>
        /// <remarks>Вызов этого метода должен выполняться драйвером подкладника
        /// при начале нового листа, начиная со ВТОРОГО</remarks>
        /// <returns>true, если отступ напечатан успешно</returns>
        protected bool PrintTopMargin()
        {
            // проверяем наличие верхнего отступа
            if (CanPrintTopMargin)
            {
                for (int i = 0; i < PrinterInfo.TopMargin - 1; i++)
                {
                    OnPrintString(LineFeedString, FontStyle.Regular);
                    if (ErrorCode.Failed)
                        return false;
                }
                OnCustomTopMarginPrinted();
                if (ErrorCode.Failed)
                    return false;
            }
            return true;
        }

        #endregion

        #region Закрытые поля и методы

        private const string badDocumentStructure = "Нарушена структура документа. Ожидалось \"{0}\", найдено \"{1}\"";
        private const string badTableStructure = "Нарушение структуры таблицы. Количество столбцов в тэге \"columns\" - {0}, обнаружено в строке - {1}";
        private const string attributeIsNotANumber = "Значение \"{0}\" атрибута {1} не является числом";
        private const string attributeIsOutOfRange = "Значение \"{0}\" атрибута {1} выходит за пределы диапазона. Ожидалось System.UInt32";

        /// <summary>
        /// Текущая ширина ленты
        /// </summary>
        private int CurrentTapeWidth
        {
            get
            {
                switch (printerNumber)
                {
                    case PrinterNumber.MainPrinter:
                        return PrinterInfo.TapeWidth.MainPrinter;
                    case PrinterNumber.AdditionalPrinter1:
                        return PrinterInfo.TapeWidth.AdditionalPrinter1;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Проверка допустимости имени элемента
        /// </summary>
        /// <param name="node">Элемент</param>
        /// <param name="expectedValue">Ожидаемое имя элемента</param>
        private void ValidateElement(XmlElement node, string expectedValue)
        {
            if (string.Compare(node.Name, expectedValue, true) != 0)
                throw new XmlException(string.Format(badDocumentStructure, expectedValue, node.Name));
        }

        /// <summary>
        /// Возвращает тип документа по значению атрибута
        /// </summary>
        /// <param name="xmlValue">Значение атрибута</param>
        private DocumentType DocTypeFromXml(string xmlValue)
        {
            switch(xmlValue)
            {
                case "sale":
                    return DocumentType.Sale;
                case "refund":
                    return DocumentType.Refund;
                case "payIn":
                    return DocumentType.PayingIn;
                case "payOut":
                    return DocumentType.PayingOut;
                case "reportX":
                    return DocumentType.XReport;
                case "reportZ":
                    return DocumentType.ZReport;
                case "reportSections":
                    return DocumentType.SectionsReport;
                case "reportFdoExchangeState":
                    return DocumentType.FDOExchangeStateReport;
                default:
                    return DocumentType.Other;
            }
        }

        /// <summary>
        /// Возвращает тип работы документа с ДЯ
        /// </summary>
        /// <param name="drawerValue">Значение атрибута</param>
        private DrawerOption DrawerOptionFromXml(string drawerValue)
        {
            switch(drawerValue)
            {
                case "openBefore":
                    return DrawerOption.OpenBefore;
                case "openAfter":
                    return DrawerOption.OpenAfter;
                default:
                    return DrawerOption.OpenNever;
            }
        }

        /// <summary>
        /// Возвращает выравнивание элемента
        /// </summary>
        /// <param name="alignOption">Значение атрибута</param>
        private AlignOptions AlignFromXml(string alignOption)
        {
            switch(alignOption)
            {
                case "right":
                    return AlignOptions.Right;
                case "center":
                    return AlignOptions.Center;
                default:
                    return AlignOptions.Left;
            }
        }

        /// <summary>
        /// Бросает исключение при неудачной конвертации значения атрибута в целое
        /// </summary>
        /// <param name="fmtMessage">Шаблон сообщения об ошибке</param>
        /// <param name="xmlAttribute">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        private void ThrowIntArgumentException(string fmtMessage, string xmlAttribute, string value)
        {
            // пишем в лог
            string message = string.Format(fmtMessage, value, xmlAttribute);
            Logger.WriteEntry(message, EventLogEntryType.Error);

            // бросаем исключение
            throw new ArgumentOutOfRangeException(xmlAttribute, value, message);
        }

        /// <summary>
        /// Возвращает целое цисло по значению атрибута
        /// </summary>
        /// <param name="lineEntry">Строка документа</param>
        /// <param name="xmlAttribute">Атрибут, значение которого будет преобразовано в целое</param>
        /// <param name="defaultValue">Значение по умолчанию, если атрибут не найден</param>
        private uint IntFromXml(XmlElement lineEntry, string xmlAttribute, uint defaultValue)
        {
            // получаем значение атрибута
            string intValue = lineEntry.GetAttribute(xmlAttribute);

            // если атрибут отсутствует, либо его значение - пустая строка
            if (string.IsNullOrEmpty(intValue))
                // возвращаем значение по умолчанию
                return defaultValue;
            else
            {
                // пытаемся преобразовать его в целое
                try
                {
                    return Convert.ToUInt32(intValue);
                }
                catch (FormatException)
                {
                    ThrowIntArgumentException(attributeIsNotANumber, xmlAttribute, intValue);
                    return 0;
                }
                catch (OverflowException)
                {
                    ThrowIntArgumentException(attributeIsOutOfRange, xmlAttribute, intValue);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Возвращает 
        /// </summary>
        /// <param name="sectionValue"></param>
        /// <returns></returns>
        private byte SectionFromXml(string sectionValue)
        {
            if (string.IsNullOrEmpty(sectionValue))
                return 1;
            else
            {
                byte section = Convert.ToByte(sectionValue);
                if (section > 0 && section < 99)
                    return section;
                else
                    throw new DeviceManagerException(string.Format("Номер секции вне диапазона", section));
            }
        }

        /// <summary>
        /// Возвращает тип оплаты по значению атрибута
        /// </summary>
        /// <param name="paymentTypeValue"></param>
        /// <returns></returns>
        private FiscalPaymentType PaymentTypeFromXml(string paymentTypeValue)
        {
            switch(paymentTypeValue)
            {
                case "card":
                    return FiscalPaymentType.Card;
                case "other1":
                    return FiscalPaymentType.Other1;
                case "other2":
                    return FiscalPaymentType.Other2;
                case "other3":
                    return FiscalPaymentType.Other3;
                default:
                    return FiscalPaymentType.Cash;
            }
        }

        private const Char Space = ' ';

        /// <summary>
        /// Проверяет, является ли исходная строка amp-строкой
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="parts">Левая и правая части amp-строки</param>
        /// <returns>True, если исходная строка является amp-строкой</returns>
        private bool IsAmpString(string source, out string[] parts)
        {
            string amp = "##";
            if (string.IsNullOrEmpty(source))
            {
                parts = null;
                return false;
            }
            else
            {
                // проверяем наличие комбинации ##
                int ampIndex = source.IndexOf(amp);
                if (ampIndex == -1)
                {
                    // исходная строка не является amp-строкой
                    parts = null;
                    return false;
                }
                else
                {
                    // исходная строка возмонжо является amp-строкой
                    parts = source.Split(new string[] { amp }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 1;
                }
            }
        }

        /// <summary>
        /// Выравнивание строки на отрезке заданной ширины
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="align">Выравнивание</param>
        /// <param name="width">Ширина отрезка</param>
        /// <param name="isBold">Жирный шрифт</param>
        /// <param name="lastSpace">Заменить последний символ на пробел</param>
        private string PrepareString(string source, AlignOptions align, int width, 
            bool isBold, bool lastSpace)
        {
            StringBuilder dest = new StringBuilder();
            int spacesCount;

            if (isBold)
                width = width / 2;

            if (lastSpace)
                width--;

            string[] ampStrParts;
            if (IsAmpString(source, out ampStrParts))
            {
                // amp-строка
                // считаем количество пробелов
                spacesCount = width - (ampStrParts[0].Length + ampStrParts[1].Length);

                if (spacesCount <= 0)
                {
                    // слишком длинная строка для заданной ширины чековой ленты
                    var longStr = string.Concat(ampStrParts[0], Space, ampStrParts[1]);

                    switch (align)
                    {
                        case AlignOptions.Right:
                            // обрезаем строку слева
                            dest.Append(longStr.Substring(longStr.Length - width));
                            break;
                        default:
                            // обрезаем строку справа
                            dest.Append(longStr.Substring(0, width));
                            break;
                    }
                }
                else
                {
                    // первая часть
                    dest.Append(ampStrParts[0]);
                    dest.Append(new string(Space, spacesCount));
                    dest.Append(ampStrParts[1]);
                }
            }
            else
            {
                // обычная строка
                if (source.Length >= width)
                {
                    // выравнивание невозможно
                    if (width > 0)
                        dest.Append(source, 0, width);
                }
                else
                    // выравниваем текст
                    switch (align)
                    {
                        case AlignOptions.Center:
                            // выравнивание по центру
                            spacesCount = (width - source.Length) / 2;
                            dest.Append(Space, spacesCount);
                            dest.Append(source);
                            dest.Append(Space, spacesCount);
                            if (dest.Length < width)
                                dest.Append(new string(Space, width - dest.Length));
                            break;
                        case AlignOptions.Right:
                            // выравнивание по правому краю
                            spacesCount = width - source.Length;
                            dest.Append(Space, spacesCount);
                            dest.Append(source);
                            break;
                        default:
                            // выравнивание по левому краю
                            spacesCount = width - source.Length;
                            dest.Append(source);
                            dest.Append(Space, spacesCount);
                            break;
                    }
            }

            if (lastSpace)
                dest.Append(Space);
            return dest.ToString();
        }

        /// <summary>
        /// Возращает стиль шрифта по значению атрибута
        /// </summary>
        /// <param name="fontStyleValue">Значение атрибута</param>
        private FontStyle FontStyleFromXml(string fontStyleValue)
        {
            switch(fontStyleValue)
            {
                case "doubleAll":
                    // проверяем, поддерживает ли принтер шрифт удвоенной ширины
                    if (PrinterInfo.SupportsBoldFont)
                        // поддерживается
                        return FontStyle.DoubleAll;
                    else
                        // не поддерживается
                        return FontStyle.Regular;

                case "doubleWidth":
                    if (PrinterInfo.SupportsBoldFont)
                        return FontStyle.DoubleWidth;
                    else
                        return FontStyle.Regular;
                case "doubleHeight":
                    return FontStyle.DoubleHeight;
                default:
                    return FontStyle.Regular;
            }
        }

        /// <summary>
        /// Печать таблицы
        /// </summary>
        /// <param name="tableEntry">XML-элемент, представляющий таблицу</param>
        private void PrintTable(XmlElement tableEntry)
        {
            FontStyle tableStyle = FontStyleFromXml(tableEntry.GetAttribute("style"));

            // определяем стили колонок
            XmlElement columns = tableEntry["columns"];
            List<ColumnAttributes> attribsList = new List<ColumnAttributes>();

            StringBuilder currentLine = new StringBuilder();
            int
                // количество колонок, для которых не задана ширина
                zeroColumns = 0, 
                // суммарное значение ширины колонок
                totalSetWidth = 0, 
                // вычисляемая ширина колонок, для которых не задана ширина
                zeroWidth = 0;

            foreach (XmlElement column in columns)
            {
                // выравнивание
                ColumnAttributes attribs = new ColumnAttributes();
                attribs.align = AlignFromXml(column.GetAttribute("align"));

                // ширина
                if (column.HasAttribute("width"))
                {
                    // задана абсолютная ширина
                    attribs.width = Convert.ToInt32(column.GetAttribute("width"));
                    totalSetWidth += attribs.width;
                }
                else 
                {
                    if (column.HasAttribute("relativeWidth"))
                    {
                        // задана относительная ширина
                        // если шрифт жирный, уменьшаем ширину в два раза
                        int tapeWidth = 
                            tableStyle == FontStyle.DoubleAll || 
                            tableStyle == FontStyle.DoubleWidth ? 
                            CurrentTapeWidth / 2 : CurrentTapeWidth;

                        attribs.width = tapeWidth * Convert.ToInt32(column.GetAttribute("relativeWidth")) / 100;
                        totalSetWidth += attribs.width;
                    }
                    else
                        // увеличиваем число колонок, для которых не задана ширина
                        zeroColumns++;
                }
                attribsList.Add(attribs);
            }

            // теперь для колонок с нулевой шириной рассчитаем ее
            if (zeroColumns > 0)
                zeroWidth = (CurrentTapeWidth - totalSetWidth) / zeroColumns;
            
            // формируем строку заголовка и проставляем ширину для тех колонок,
            // у которых она была равна нулю
            for (int i = 0; i < columns.ChildNodes.Count; i++)
            {
                ColumnAttributes attribs = attribsList[i];
                if (attribs.width == 0)
                    attribs.width = zeroWidth;

                // добавляем заголовок колонки в список
                XmlNode column = columns.ChildNodes[i];
                currentLine.Append(PrepareString(column.InnerText, attribs.align, attribs.width,
                    false, column != column.ParentNode.LastChild));
            }

            // сетка таблицы
            GridOption grid = GridOption.None;
            string gridLine = string.Empty;
            if (tableEntry.HasAttribute("grid"))
            {
                // сетка есть
                grid = GridOptionFromXml(tableEntry.GetAttribute("gridOptions"));
                gridLine = BuildCustomSeparator(tableEntry.GetAttribute("grid"));

                // проверяем, нужно ли печатать верхнюю границу шапки
                if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Top) == GridOption.Top)
                {
                    OnPrintString(gridLine, FontStyle.Regular);
                    if (ErrorCode.Failed)
                        return;
                }
            }

            string phAttr = tableEntry.GetAttribute("printHeader");
            if (string.IsNullOrEmpty(phAttr) || string.Compare(phAttr, "true", true) == 0)
            {
                // печатем заголовок таблицы
                if (!PrintCurrentLine(currentLine, tableStyle))
                    return;
            }

            // проверяем, нужно ли печатать границу между шапкой и телом таблицы
            if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Middle) == GridOption.Middle)
            {
                OnPrintString(gridLine, FontStyle.Regular);
                if (ErrorCode.Failed)
                    return;
            }

            // печать строк
            foreach (XmlElement row in tableEntry["rows"])
            {
                // проверяем количество полей в очередной записи таблицы
                if (row.ChildNodes.Count != columns.ChildNodes.Count)
                    throw new XmlException(string.Format(badTableStructure, 
                        columns.ChildNodes.Count, row.ChildNodes.Count));

                // формируем очередную строку
                for (int i = 0; i < row.ChildNodes.Count; i++)
                {
                    XmlNode currentNode = row.ChildNodes[i];
                    currentLine.Append(PrepareString(currentNode.InnerText, attribsList[i].align, 
                        attribsList[i].width, false, currentNode != currentNode.ParentNode.LastChild));
                }

                // вывод на печать очередной строки
                if (!PrintCurrentLine(currentLine, tableStyle))
                    return;
            }

            // проверяем, нужно ли печатать нижнюю граница тела таблицы
            if ((grid & GridOption.All) == GridOption.All || (grid & GridOption.Bottom) == GridOption.Bottom)
                OnPrintString(gridLine, FontStyle.Regular);
        }

        /// <summary>
        /// Вовзвращает параметры печати сетки
        /// </summary>
        /// <param name="gridOptionValue"></param>
        /// <returns></returns>
        private GridOption GridOptionFromXml(string gridOptionValue)
        {
            if (string.IsNullOrEmpty(gridOptionValue))
                return GridOption.All;

            string[] options = gridOptionValue.Split('|');
            GridOption result = GridOption.None;
            foreach (string option in options)
            {
                switch(option)
                {
                    case "top":
                        result = result | GridOption.Top;
                        break;
                    case "middle":
                        result = result | GridOption.Middle;
                        break;
                    case "bottom":
                        result = result | GridOption.Bottom;
                        break;
                    default:
                        result = result | GridOption.All;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Печать текущей строки таблицы
        /// </summary>
        /// <param name="currentLine">Текущая строка</param>
        /// <param name="tableStyle">Стиль шрифта</param>
        private bool PrintCurrentLine(StringBuilder currentLine, FontStyle tableStyle)
        {
            if (currentLine.Length > CurrentTapeWidth)
                OnPrintString(currentLine.ToString().Substring(0, CurrentTapeWidth), tableStyle);
            else
                OnPrintString(currentLine.ToString(), tableStyle);
            currentLine.Length = 0;
            return ErrorCode.Succeeded;
        }

        /// <summary>
        /// Возвращает строку-разделитель, сформированную по заданному шаблону
        /// </summary>
        /// <param name="source">Шаблон</param>
        private string BuildCustomSeparator(string source)
        {
            if (string.IsNullOrEmpty(source))
                // разделитель формируется из параметров устройства
                return new string(Separator, CurrentTapeWidth);

            // разделитель задан шаблоном
            if (source.Length >= CurrentTapeWidth)
                // шаблон слишком длинный
                return source.Substring(0, CurrentTapeWidth);

            // шаблон умешается в строке более одного раза
            StringBuilder separator = new StringBuilder();
            while (separator.Length + source.Length <= CurrentTapeWidth)
                separator.Append(source);

            // проверим, есть ли свободное место в конце строки
            if (separator.Length < CurrentTapeWidth)
                separator.Append(source.Substring(0, CurrentTapeWidth - separator.Length));

            return separator.ToString();
        }

        /// <summary>
        /// Можно ли печатать верхний отступ
        /// </summary>
        private bool CanPrintTopMargin
        {
            get
            {
                // верхний отступ (если задан), печатается:
                // 1) если принтер чековый - на чековой ленте
                // 2) если прнитер подкладник - на подкладном документе
                // 3) если принтер комбинированный - ТОЛЬКО НЕ на чековой ленте

                if (PrinterInfo.TopMargin == 0)
                    // отступ не задан
                    return false;

                switch (PrinterInfo.Kind)
                {
                    case PrinterKind.Combo:
                        return printerNumber != PrinterNumber.MainPrinter; 
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// Печать отдельного документа.
        /// </summary>
        /// <param name="docType">
        /// Тип документа.
        /// </param>
        /// <param name="docEntry">
        /// Контент документа.
        /// </param>
        private bool PrintDocumentEntry(DocumentType docType, XmlElement docEntry)
        {
            // печатаем верхний отступ
            if (!PrintTopMargin())
                return false;

            var wasNewRegistrations = false;
            var trimDocumentAmountWasCalled = false;
            uint newRegistrationsAmount = 0;

            // идем по всем строкам документа
            foreach(XmlElement lineEntry in docEntry)
            {
                // получаем данные строки
                string lineData = lineEntry.InnerText;

                // определяем тип строки
                switch (lineEntry.GetAttribute("type"))
                {
                    case "separator":
                        // строка-разделитель
                        OnPrintString(BuildCustomSeparator(lineData), FontStyle.Regular);
                        break;

                    case "barcode":
                        // штрих-код
                        string isReadable = lineEntry.GetAttribute("readable");
                        OnPrintBarcode(lineData, AlignFromXml(lineEntry.GetAttribute("align")),
                            isReadable == "true" || string.IsNullOrEmpty(isReadable));
                        break;

                    case "image":
                        // рисунок
                        byte[] imageBytes = Encoding.Default.GetBytes(lineData);
                        using(MemoryStream imageStream = new MemoryStream(imageBytes))
                        {
                            OnPrintImage(new System.Drawing.Bitmap(imageStream), 
                                AlignFromXml(lineEntry.GetAttribute("align")));
                        }
                        break;

                    case "registration":
                        // регистрация;
                        // попытаемся понять, какой способ регистрации нам нужен - старый либо по 54-ФЗ
                        if (MustUseNewRegistrationMethod(docType, lineEntry))
                        {
                            wasNewRegistrations = true;

                            var amount = IntFromXml(lineEntry, "amount", 0);

                            newRegistrationsAmount += amount;

                            if (string.IsNullOrEmpty(lineEntry.GetAttribute("fiscalItemType")))
                            {
                                // ФФД 1.0
                                OnRegistration(
                                    lineData,
                                    IntFromXml(lineEntry, "quantity", 1000),
                                    IntFromXml(lineEntry, "price", 0),
                                    amount,
                                    SectionFromXml(lineEntry.GetAttribute("section")),
                                    (byte)IntFromXml(lineEntry, "vatRateId", 1));
                            }
                            else
                            {
                                OnRegistration(
                                    lineData,
                                    IntFromXml(lineEntry, "quantity", 1000),
                                    IntFromXml(lineEntry, "price", 0),
                                    amount,
                                    SectionFromXml(lineEntry.GetAttribute("section")),
                                    (byte)IntFromXml(lineEntry, "vatRateId", 1),
                                    (byte)IntFromXml(lineEntry, "fiscalItemType", 1));
                            }
                        }
                        else
                        {
                            OnRegistration(lineData,
                                IntFromXml(lineEntry, "quantity", 1000),
                                IntFromXml(lineEntry, "amount", 0),
                                SectionFromXml(lineEntry.GetAttribute("section")));
                        }
                        break;

                    case "payment":
                        // оплата
                        if (wasNewRegistrations && !trimDocumentAmountWasCalled)
                        {
                            // идет формирование фискального документа по 54-ФЗ, нужно проверить,
                            // не требуется ли выравнивание суммы чека
                            OnTrimDocumentAmount(newRegistrationsAmount);
                            trimDocumentAmountWasCalled = true;
                        }

                        OnPayment(IntFromXml(lineEntry, "amount", 0), PaymentTypeFromXml(lineEntry.GetAttribute("paymentType")));
                        break;

                    case "cash":
                        // внесение или выплата
                        OnCash(IntFromXml(lineEntry, "amount", 0));
                        break;

                    case "table":
                        // таблица
                        PrintTable(lineEntry);
                        break;

                    default:
                        // просто текст
                        FontStyle style = FontStyleFromXml(lineEntry.GetAttribute("style"));

                        string s = PrepareString(
                            lineData, 
                            AlignFromXml(lineEntry.GetAttribute("align")),
                            CurrentTapeWidth, 
                            style == FontStyle.DoubleAll || style == FontStyle.DoubleWidth, 
                            false);

                        OnPrintString(s, style);
                        break;
                }

                // если печать очередной строки завершилась неудачно,
                // прерываем разбор документа и выходим
                if (ErrorCode.Failed)
                    return false;
            }

            // возвращаем результат печати
            return true;
        }

        private bool MustUseNewRegistrationMethod(DocumentType docType, XmlElement lineEntry)
        {
            return
                !string.IsNullOrEmpty(lineEntry.GetAttribute("vatRateId")) &&
                (docType == DocumentType.Sale || docType == DocumentType.Refund);
        }

        /// <summary>
        /// Проверка существования строк типа "registration" c ненулевой суммой
        /// </summary>
        /// <param name="docEntry">Корневой элемент документа</param>
        private void InitHasNonzeroRegistrations(XmlElement docEntry)
        {
            hasNonzeroRegistrations = false;

            // идем по всем строкам документа
            foreach (XmlElement lineEntry in docEntry)
            {
                // проверяем корректность имени строки
                ValidateElement(lineEntry, "line");

                // определяем тип строки
                if (lineEntry.GetAttribute("type") == "registration")
                {
                    // проверяем сумму
                    if (IntFromXml(lineEntry, "amount", 0) != 0)
                    {
                        // есть регистрация с ненулевой суммой
                        hasNonzeroRegistrations = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Инициализиция поля "номер принтера"
        /// </summary>
        /// <param name="xmlValue">Значение атрибута, загруженное из XML-документа</param>
        private void InitPrinterNumber(string xmlValue)
        {
            switch (xmlValue)
            {
                case "additionalPrinter1":
                    printerNumber = PrinterNumber.AdditionalPrinter1;
                    break;
                case "additionalPrinter2":
                    printerNumber = PrinterNumber.AdditionalPrinter2;
                    break;
                default:
                    printerNumber = PrinterNumber.MainPrinter;
                    break;
            }
        }

        #endregion

        #region Реализация IPrintableDevice

        #region Шапка и подвал документа

        /// <summary>
        /// Заголовок документа
        /// </summary>
        public string[] DocumentHeader
        {
            get
            {
                return documentHeader.ToArray();
            }
            set
            {
                documentHeader.Clear();
                if (value != null && value.Length > 0)
                    documentHeader.AddRange(value);
            }
        }

        /// <summary>
        /// Подвал документа
        /// </summary>
        public string[] DocumentFooter		
        {
            get
            {
                return documentFooter.ToArray();
            }
            set
            {
                documentFooter.Clear();
                if (value != null && value.Length > 0)
                    documentFooter.AddRange(value);
            }
        }

        /// <summary>
        /// Графический заголовок документа
        /// </summary>
        public System.Drawing.Bitmap GraphicHeader 
        { 
            get { return _graphicHeader; }
            set { _graphicHeader = value; }
        }

        /// <summary>
        /// Графический подвал документа
        /// </summary>
        public System.Drawing.Bitmap GraphicFooter 
        { 
            get { return _graphicFooter; }
            set { _graphicFooter = value; }
        }

        /// <summary>
        /// Печатать графический заголовок чека
        /// </summary>
        public bool PrintGraphicHeader 
        {
            get { return _printGraphicHeader; }
            set { _printGraphicHeader = value; }
        }

        /// <summary>
        /// Печатать графический подвал документа
        /// </summary>
        public bool PrintGraphicFooter 
        { 
            get { return _printGraphicFooter; }
            set { _printGraphicFooter = value; }
        }

        /// <summary>
        /// Печатать заголовок документа
        /// </summary>
        public bool PrintHeader 
        { 
            get { return _printHeader; }
            set { _printHeader = value; }
        }

        /// <summary>
        /// Печатать подвал документа
        /// </summary>
        public bool PrintFooter 
        { 
            get { return _printFooter; }
            set { _printFooter = value; }
        }

        #endregion

        /// <summary>
        /// Печать документа
        /// </summary>
        /// <param name="xmlData">Данные XML-документа</param>
        public virtual void Print(string xmlData)
        {
            DrawerOption drwOption;
            DocumentType docType;
            bool printResult = false;

            // если включен режим отладки
            if (Logger.DebugInfo)
            {
                // сохраняем отладочную информацию
                Logger.WriteEntry(this, "Сохранение отладочной информации", EventLogEntryType.Information);
                Logger.SaveDebugInfo(this, xmlData);
            }

            // проверяем статус устройства
            if (!Active)
            {
                // устройство не активно
                ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                return;
            }

            // проверим, не совпадает ли наш текущий документ с предыдущим,
            // отправленным на печать
            if (string.Compare(xmlData, previousXmlData, false) == 0 && 
                PrinterStatus.PaperOut == PaperOutStatus.OutAfterActive)
            {
                // подаем команду продолжения печати
                Logger.WriteEntry(this, "Продолжение печати предыдущего документа", EventLogEntryType.Information);
                OnContinuePrint();
                // проверим, есть ли открытый документ
                if (!PrinterStatus.OpenedDocument)
                    // открытого документа нет, значит, был допечатан документ,
                    // отменить который было невозможно
                    // выходим
                    return;
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlData);
            Logger.WriteEntry(this, "Данные документа загружены", EventLogEntryType.Information);

            // кореневой элемент
            XmlElement root = document.DocumentElement;
            ValidateElement(root, "documents");
            foreach(XmlElement docEntry in root)
            {
                // очередной документ
                Logger.WriteEntry(this, "Начало печати документа", EventLogEntryType.Information);
                ValidateElement(docEntry, "document");
                // определяем тип документа
                docType = DocTypeFromXml(docEntry.GetAttribute("type"));
                // инициализируем номер принтера
                InitPrinterNumber(docEntry.GetAttribute("printer"));
                // инициализируем свойство HasNonzeroRegistrations
                InitHasNonzeroRegistrations(docEntry);

                // открываем документ
                var cashier = docEntry.GetAttribute("cashier");
                var cashierInn = docEntry.GetAttribute("cashierInn");
                var customerPhoneOrEmail = docEntry.GetAttribute("customerPhoneOrEMail");

                OnOpenDocument(docType, cashier, cashierInn, customerPhoneOrEmail);

                if (ErrorCode.Failed)
                    // если открытие документа выполнено с ошибкой, прерываем печать 
                    // всех документов на принтере
                    break;
                try
                {
                    drwOption = DrawerOptionFromXml(docEntry.GetAttribute("drawer"));
                    if (drwOption == DrawerOption.OpenBefore)
                    {
                        // открываем ДЯ до печати документа
                        OnOpenDrawer();
                        printResult = ErrorCode.Succeeded;
                        if (!printResult)
                            break;
                    }

                    // печать тела документа
                    printResult = PrintDocumentEntry(docType, docEntry);
                    if (!printResult)
                        break;

                    if (drwOption == DrawerOption.OpenAfter)
                    {
                        // открываем ДЯ после печати документа
                        OnOpenDrawer();
                        printResult = ErrorCode.Succeeded;
                        if (!printResult)
                            break;
                    }
                }
                finally
                {
                    // закрываем документ
                    if (printResult)
                    {
                        // определяем, нужно ли произвести отрезку чека
                        var cutAttribute = docEntry.GetAttribute("cut");
                        var cutPaper = cutAttribute == "true" || string.IsNullOrEmpty(cutAttribute);

                        // закрываем документ
                        OnCloseDocument(cutPaper);
                    }
                }
                Logger.WriteEntry(this, "Печать документа успешно завершена", EventLogEntryType.Information);
            }

            // сохраняем документ
            previousXmlData = xmlData;
        }

        /// <summary>
        /// Событие, возникаемое при необходимости реакции пользователя
        /// на состояние печатающего устройства
        /// </summary>
        public abstract event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        /// <summary>
        /// Аппаратные характеристики печатающего устройства
        /// </summary>
        public abstract PrintableDeviceInfo PrinterInfo { get; }

        /// <summary>
        /// Флаги состояния принтера
        /// </summary>
        public abstract PrinterStatusFlags PrinterStatus { get; }

        /// <summary>
        /// Символ-разделитель логических секций документа
        /// </summary>
        public char Separator
        {
            get { return separator;	}
            set { separator = value; }
        }

        /// <summary>
        /// Открытие денежного ящика
        /// </summary>
        public void OpenDrawer()
        {
            OnOpenDrawer();
        }

        #endregion
    }
}
