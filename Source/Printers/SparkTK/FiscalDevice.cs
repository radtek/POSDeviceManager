using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace SparkTK
{
    internal enum DocumentState { None, Fiscal, NonFiscal }

    /// <summary>
    /// Драйвер ФР "Прим", работающих по протоколу первых версий (без команды печати строки). Печать 
    /// произвольных строк производится в режиме принтера. Текущий режим (команды принтера или 
    /// протокольные команды ФР) определяеся с помощью короткого запроса состояния принтера
    /// </summary>
    [Serializable]
    [FiscalDevice(DeviceNames.ecrTypeSpark)]
    public class SparkTKFiscalDevice : CustomFiscalDevice, ISparkDeviceProvider
    {
        #region Константы

        // максимальная длина строки (символов)
        private const int MAX_STRING_LEN = 40;

        // таймаут чтения (мс.)
        private const int READ_TIMEOUT = 5000;

        // таймаут записи (мс.)
        private const int WRITE_TIMEOUT = 2000;

        // сообщение об отмене документа
        private const string CANCEL_DOCUMENT = "ДОКУМЕНТ АННУЛИРОВАН";

        #endregion

        #region Поля

        private NumberFormatInfo _currNfi = new NumberFormatInfo();

        // переменная для эмуляции регистра суммы открытого документа
        private uint _documentAmount = 0;

        // тип текущего документа
        private DocumentType _currDocType;

        // состояние документа
        private bool _docOpened;

        // имя кассира
        private string _cashierName;

        private SparkProtocol _deviceProtocol = null;

        private string _versNo;

        private ulong _storedCashInDrawer = 0;

        private bool _isSparkOld = false;

        #endregion

        #region Свойства

        /// <summary>
        /// Устройство поддерживает печать произвольных строк документах любого типа. Режим принтера
        /// не используется
        /// </summary>
        protected virtual bool IsPrim02
        {
            get { return false; }
        }

        /// <summary>
        /// Печать произвольных строк производится в режиме принтера. Текущий режим (команды принтера или 
        /// протокольные команды ФР) невозможно получить от устройства. Поэтому текущий режим хранится в 
        /// данных драйвера
        /// </summary>
        protected virtual bool IsSparkOld
        {
            get { return _isSparkOld; }
        }

        #endregion

        #region Конструктор

        public SparkTKFiscalDevice()
            : base()
        {
            _currNfi.NumberDecimalSeparator = ".";
            _currNfi.NumberGroupSeparator = "";

            AddSpecificError(0x01, "Неверный формат сообщения");
            AddSpecificError(0x02, "Неверный формат поля");
            AddSpecificError(0x03, "Неверные дата/время. Невозможно установить переданную дату/время");
            AddSpecificError(0x04, "Неверная контрольная сумма");
            AddSpecificError(0x05, "Неверный пароль передачи данных");
            AddSpecificError(0x06, "Нет команды с таким номером");
            AddSpecificError(0x07, "Необходима команда \"Начало сеанса\"");
            AddSpecificError(0x08, "Время изменилось больше, чем на 24 часа");
            AddSpecificError(0x09, "Превышена максимальная длина строкового поля");
            AddSpecificError(0x0A, "Превышена максимальная длина сообщения");
            AddSpecificError(0x0B, "Неправильная операция");
            AddSpecificError(0x0C, "Значение поля вне диапазона");
            AddSpecificError(0x0D, "При данном состоянии документа эта команда недопустима");
            AddSpecificError(0x0E, "Обязательное строковое поле имеет нулевую длину");
            AddSpecificError(0x0F, "Слишком большой результат");
            AddSpecificError(0x10, "Переполнение денежного счетчика");
            AddSpecificError(0x11, "Обратная операция невозможна из-за отстутствия прямой");
            AddSpecificError(0x12, "Нет столько наличных для выполнения операции");
            AddSpecificError(0x13, "Обратная операция превысила итого по прямой операции");
            AddSpecificError(0x14, "Необходимо выполнить сертификацию (ввод заводского номера)");
            AddSpecificError(0x15, "Необходимо выполнить закрытие смены");
            AddSpecificError(0x16, "Таймаут при печати");
            AddSpecificError(0x17, "Неисправимая ошибка принтера");
            AddSpecificError(0x18, "Принтер не готов к печати");
            AddSpecificError(0x19, "Бумага близка к концу");
            AddSpecificError(0x1A, "Необходимо провести фискализацию");
            AddSpecificError(0x1B, "Неверный пароль доступа к ФП");
            AddSpecificError(0x1C, "ККМ уже сертифицирована");
            AddSpecificError(0x1D, "Исчерпано число фискализаций");
            AddSpecificError(0x1E, "Неверный буфер печати");
            AddSpecificError(0x1F, "Неверное G-поле");
            AddSpecificError(0x20, "Неверный номер типа оплаты");
            AddSpecificError(0x21, "Таймаут приема");
            AddSpecificError(0x22, "Ошибка приема");
            AddSpecificError(0x23, "Неверное состояние ККМ");
            AddSpecificError(0x24, "Слишком много операций в документе. Необходима команда \"Аннулировать\"");
            AddSpecificError(0x25, "Необходима команда \"Открытие смены\"");
            AddSpecificError(0x27, "Неверный номер вида платежа");
            AddSpecificError(0x28, "Неверное состояние принтера");
            AddSpecificError(0x29, "Смена уже открыта");
            AddSpecificError(0x2B, "Неверная дата");
            AddSpecificError(0x2C, "Нет места для добавления отдела/составляющей");
            AddSpecificError(0x2D, "Индекс отдела/составляющей уже существует");
            AddSpecificError(0x2E, "Невозможно удалить отдел - есть составляющие");
            AddSpecificError(0x2F, "Индекс отдела/составляющей не обнаружен");
            AddSpecificError(0x30, "Фискальная память не исправна");
            AddSpecificError(0x31, "Дата последней существующей записи в ФП позже, чем дата операции");
            AddSpecificError(0x32, "Необходима инициализация ФП");
            AddSpecificError(0x33, "Заполнена вся ФП");
            AddSpecificError(0x34, "Некорректный стартовый символ на приеме");
            AddSpecificError(0x35, "Неопознанный ответ от ЭКЛЗ");
            AddSpecificError(0x36, "Неизвестная команда ЭКЛЗ");
            AddSpecificError(0x37, "Неверное состояние ЭКЛЗ");
            AddSpecificError(0x38, "Таймаут приема от ЭКЛЗ");
            AddSpecificError(0x39, "Таймаут передачи в ЭКЛЗ");
            AddSpecificError(0x3A, "Неверная контрольная сумма ответа ЭКЛЗ");
            AddSpecificError(0x3B, "Аварийное состояние ЭКЛЗ");
            AddSpecificError(0x3C, "Нет свободного места в ЭКЛЗ");
            AddSpecificError(0x3D, "Неверная контрольная сумма в команде ЭКЛЗ");
            AddSpecificError(0x3E, "Контроллер ЭКЛЗ не обнаружен");
            AddSpecificError(0x3F, "Данные в ЭКЛЗ отсутствуют");
            AddSpecificError(0x40, "Данные в ЭКЛЗ не синхронизированы");
            AddSpecificError(0x41, "Аварийное состояние РИК");
            AddSpecificError(0x42, "Неверные дата и время в команде ЭКЛЗ");
            AddSpecificError(0x43, "Закончилось время эксплуатации ЭКЛЗ");
            AddSpecificError(0x44, "Переполнение ЭКЛЗ");
            AddSpecificError(0x45, "Число активизаций исчерпано");
            AddSpecificError(0x50, "Некорректное состояние контрольной ленты");
            AddSpecificError(0x51, "Требуется распечатка контрольной ленты");
            AddSpecificError(0xFE, "Документ не открыт");
            AddSpecificError(0xFF, "Неизвестная ошибка");
            AddSpecificError(0xFC, "Включен режим принтера");

            _deviceProtocol = new SparkProtocol(this);
            //this.Logger.SaveDebugInfo()
        }

        #endregion

        private delegate void ExecuteCommandDelegate();

        #region Внутренние методы

        // установка заголовка документа
        private void SetHeader()
        {
            if (DocumentHeader != null && PrintHeader)
            {
                _deviceProtocol.CreateCommand("4E");
                for (int i = 0; i < 6; i++)
                    if (i >= DocumentHeader.Length)
                        _deviceProtocol.AddString("");
                    else
                        _deviceProtocol.AddString(DocumentHeader[i].Length > MAX_STRING_LEN ? DocumentHeader[i].Substring(0, MAX_STRING_LEN) : DocumentHeader[i]);

                _deviceProtocol.Execute();
            }

            // графический заголовок 4F
            if (GraphicHeader != null && PrintGraphicHeader)
            {
                int width = GraphicHeader.Width / 8;
                int height = GraphicHeader.Height / 8;
                _deviceProtocol.CreateCommand("4F");
                // тип заголовка
                _deviceProtocol.AddString("00");
                // размер по горизонтали (в байтах)
                _deviceProtocol.AddString(width.ToString("X2"));
                // размер по вертикали (в байтах)
                _deviceProtocol.AddString(height.ToString("X2"));
                // выполнение команды
                _deviceProtocol.Execute();

                // передача данных
                foreach (byte imageByte in GetImageBytes(GraphicHeader, width, height))
                    _deviceProtocol.WriteByte(imageByte);

                // чтение ответа на команду
                _deviceProtocol.GetCommandRequest();
            }
        }

        // установка подвала документа
        private void SetFooter()
        {
            if (DocumentHeader == null)
                return;

            _deviceProtocol.CreateCommand("46", true);
            for (int i = 0; i < 4; i++)
                if (i >= DocumentFooter.Length)
                    _deviceProtocol.AddString("");
                else
                    _deviceProtocol.AddString(DocumentFooter[i].Length > MAX_STRING_LEN ? DocumentFooter[i].Substring(0, MAX_STRING_LEN) : DocumentFooter[i]);

            _deviceProtocol.Execute();
        }

        /// <summary>
        /// Конвертирование битмапа в байты в формате Прим ФР для установки графического клише
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private IEnumerable<byte> GetImageBytes(System.Drawing.Bitmap bitmap, int width, int height)
        {
            for (int x = 0; x < width * 8; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte b = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if ((uint)bitmap.GetPixel(x, y * 8 + i).ToArgb() != 0xffffffff)
                            b += (byte)(0x01 << (7 - i));
                    }
                    yield return b;
                }
            }
        }


        // открытие фискального документа
        private void OpenFiscalDocument()
        {
            _deviceProtocol.CreateCommand("10", true);
            _deviceProtocol.AddString(_currDocType == DocumentType.Sale ? "00" : "02");
            _deviceProtocol.AddString(_cashierName);
            _deviceProtocol.AddString("");      // поля только для 
            _deviceProtocol.AddString("");      // отелей и ресторанов
            _deviceProtocol.AddString("01");    // количество копий
            _deviceProtocol.AddString("");      // номер счета
            _deviceProtocol.Execute();
        }

        // отмена документа
        private void CancelDocument(bool printerMode)
        {
            try
            {
                if (printerMode)
                    CloseNonFiscalDocument(true);
                else
                {
                    switch (_deviceProtocol.GetDeviceInfo().DocState)
                    {
                        case FiscalDocState.Closed:
                            // а если документ не открывался и режим принтера не включен,
                            // то скорее всего ничего еще не напечатано и отменять нечего
                            break;
                        case FiscalDocState.FreeDoc:
                            // закрытие произвольного документа
                            _deviceProtocol.ExecuteCommand("52");
                            break;
                        default:
                            // отмена фискального документа
                            _deviceProtocol.ExecuteCommand("17", true);
                            break;
                    }
                }
                _docOpened = false;
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _deviceProtocol.GetCommandDump());
            }
            catch (PrintableErrorException)
            {
                // ошибка печатающего устройства. документ не отменяется
                ErrorCode = new ServerErrorCode(this, 0x18, GetSpecificDescription(0x18), _deviceProtocol.GetCommandDump());
            }
            catch (DeviceErrorException E)
            {
                if (E.ErrorCode != 13)
                    ErrorCode = new ServerErrorCode(this, E.ErrorCode, GetSpecificDescription(E.ErrorCode), _deviceProtocol.GetCommandDump());
                _docOpened = false;
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !string.IsNullOrEmpty(_deviceProtocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
//                _deviceProtocol.ClearDebugInfo();
            }
        }

        // закрытие нефискального документа
        private void CloseNonFiscalDocument(bool bCancelDoc)
        {
            if (bCancelDoc)
                PrintStringInternal(CANCEL_DOCUMENT, FontStyle.Regular);

            // печать подвала
            if (DocumentFooter != null)
                foreach (string s in DocumentFooter)
                    PrintStringInternal(s, FontStyle.Regular);

            // отрезка чека
            _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('V'), 65, 0 });

            // печать клише
            if (DocumentHeader != null)
                foreach (string s in DocumentHeader)
                    PrintStringInternal(s, FontStyle.Regular);

            _deviceProtocol.IsPrinterMode = false;
            _docOpened = false;
        }

        // закрытие документа внесения или инкассации
        private void ClosePayInOutDocument()
        {
            decimal fAmount = (decimal)(_documentAmount / 100.0);
            _deviceProtocol.CreateCommand(_currDocType == DocumentType.PayingIn ? "32" : "33", true);
            _deviceProtocol.AddString(fAmount.ToString("f2", _currNfi));
            _deviceProtocol.Execute();
        }

        private void PrintStringInternal(string source, FontStyle style)
        {
            if (IsPrim02) // печать строки в открытый документ
            {
                // устанавливаем шрифт
                switch (style)
                {
                    case FontStyle.DoubleHeight:
                        source = "~10" + source;
                        break;
                    case FontStyle.DoubleWidth:
                        source = "~20" + source;
                        break;
                    case FontStyle.DoubleAll:
                        source = "~30" + source;
                        break;
                }

                // обрезаем строку до максимальной длины
                if (source.Length > MAX_STRING_LEN)
                    source = source.Substring(0, MAX_STRING_LEN);

                _deviceProtocol.CreateCommand((_currDocType != DocumentType.Other) && HasNonzeroRegistrations ? "1C" : "51");
                _deviceProtocol.AddString(source);
                _deviceProtocol.Execute();
            }
            else         // печать строки в режиме принтера
            {
                // обрезаем строку до максимальной длины
                if (source.Length > MAX_STRING_LEN)
                    source = source.Substring(0, MAX_STRING_LEN);

                // установка шрифта
                switch (style)
                {
                    case FontStyle.Regular:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x00 });
                        break;
                    case FontStyle.DoubleHeight:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x10 });
                        break;
                    case FontStyle.DoubleWidth:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x20 });
                        break;
                    case FontStyle.DoubleAll:
                        _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x30 });
                        break;
                }

                // печать строки
                _deviceProtocol.Write(Encoding.GetEncoding(866).GetBytes(source));
                _deviceProtocol.Write(new byte[] { 0x0A });

                // сброс шрифта
                _deviceProtocol.Write(new byte[] { 0x1B, 0x21, 0x00 });
            }
        }

        private void ExecuteDriverCommand(ExecuteCommandDelegate executeCommandDelegate)
        {
            ExecuteDriverCommand(false, executeCommandDelegate);
        }

        private void ExecuteDriverCommand(bool printerMode, ExecuteCommandDelegate executeCommandDelegate)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            try
            {
                if (!Active)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                    return;
                }

                if(!IsPrim02)
                    // проверяем текущий режим 
                    if (_deviceProtocol.IsPrinterMode != printerMode)
                        // устанавливаем нужный режим
                        _deviceProtocol.IsPrinterMode = printerMode;

                executeCommandDelegate();
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _deviceProtocol.GetCommandDump());
                if (_deviceProtocol.IsPrinterMode)
                    _deviceProtocol.IsPrinterMode = false;
            }
            catch (PrintableErrorException)
            {
                // Ошибка печатающего устройства. Документ не отменяется
                ErrorCode = new ServerErrorCode(this, 0x18, GetSpecificDescription(0x18), _deviceProtocol.GetCommandDump());                
            }
            catch (DeviceErrorException E)
            {
                // Протокольная ошибка. Документ нужно попробовать отменить
                ErrorCode = new ServerErrorCode(this, E.ErrorCode, GetSpecificDescription(E.ErrorCode), _deviceProtocol.GetCommandDump());
                CancelDocument(_deviceProtocol.IsPrinterMode);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !string.IsNullOrEmpty(_deviceProtocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
//                _deviceProtocol.ClearDebugInfo();
            }
        }

        #endregion

        #region Реализация виртуальных функций

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            DCB dcb = e.DCB;

            // задаем новые параметры обмена
            dcb.XonChar = 0;
            dcb.XoffChar = 0;
            dcb.XonLim = 0;
            dcb.XoffLim = 0;

            // shake
            dcb.fDsrSensitivity = 0;
            dcb.fOutxCtsFlow = 0;
            dcb.fTXContinueOnXoff = 0;
            dcb.fInX = 0;
            dcb.fOutX = 0;
            dcb.fRtsControl = (uint)RtsControl.Disable;
            dcb.fNull = 0;

            // replace
            dcb.fDtrControl = (uint)DtrControl.Handshake;
            dcb.fOutxDsrFlow = 1;
            dcb.fAbortOnError = 1;

            e.DCB = dcb;
            e.Handled = true;

            base.SetCommStateEventHandler(sender, e);
        }

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            _deviceProtocol.WriteDebugLine(string.Format("OnAfterActivate({0}, {1})", PortName, Baud));
            ExecuteDriverCommand(delegate()
            {
                _deviceProtocol.IsPrinterMode = false;

                // открыть сеанс
                _deviceProtocol.ExecuteCommand("01", true);

                if (!Status.OpenedShift)
                {
                    // установка клише
                    SetHeader();

                    // установка подвала
                    SetFooter();
                }

                // получаем версию прошивки
                _deviceProtocol.ExecuteCommand("97");
                _versNo = Encoding.ASCII.GetString(_deviceProtocol.GetField(2));
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            _deviceProtocol.ClearDebugInfo();
            _deviceProtocol.WriteDebugLine(string.Format("OnOpenDocument({0}, {1})", docType, cashierName));
            ExecuteDriverCommand(delegate()
            {
                _currDocType = docType;
                _cashierName = (cashierName.Length <= 15) ? cashierName : cashierName.Substring(0, 15);

                // если документ уже открыт, его надо отменить
                if (_deviceProtocol.GetDeviceInfo().DocState != FiscalDocState.Closed)
                    CancelDocument(false);

                if (IsPrim02)
                {
                    switch (docType)
                    {
                        case DocumentType.Sale:
                        case DocumentType.Refund:
                            if (HasNonzeroRegistrations)
                                OpenFiscalDocument();
                            else
                                // открытие нефискального документа
                                _deviceProtocol.ExecuteCommand("50", true);
                            break;
                        case DocumentType.Other:
                            // открытие нефискального документа
                            _deviceProtocol.ExecuteCommand("50", true);
                            break;
                    }
                }

                _storedCashInDrawer = Status.CashInDrawer;
                _documentAmount = 0;
                _docOpened = true;
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnCloseDocument({0})", cutPaper));
            ExecuteDriverCommand(_deviceProtocol.IsPrinterMode, delegate()
            {
                switch (_currDocType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                    case DocumentType.Other:
                        if (_deviceProtocol.IsPrinterMode)
                            CloseNonFiscalDocument(false);
                        else
                        {
                            // закрытие фискального документа
                            _storedCashInDrawer = Status.CashInDrawer;
                            _deviceProtocol.ExecuteCommand(
                                _currDocType != DocumentType.Other && HasNonzeroRegistrations ? "14" : "52");
                        }
                        break;

                    case DocumentType.PayingIn:
                    case DocumentType.PayingOut:
                        ClosePayInOutDocument();
                        break;

                    case DocumentType.SectionsReport:
                    case DocumentType.XReport:
                        _deviceProtocol.ExecuteCommand("30", true);
                        break;
                    case DocumentType.ZReport:
                        _deviceProtocol.ExecuteCommand("31", true);
                        break;
                }

                _docOpened = false;
                _currDocType = DocumentType.Other;
                _documentAmount = 0;
            });
        }

        protected override void OnOpenDrawer()
        {
            _deviceProtocol.WriteDebugLine("OnOpenDrawer");
            ExecuteDriverCommand(!IsPrim02 && _deviceProtocol.IsPrinterMode, delegate()
            {
                _deviceProtocol.Write(new byte[] { 0x05 });
                System.Threading.Thread.Sleep(1000);
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPrintString({0}, {1})", source, style));
            ExecuteDriverCommand(!IsPrim02, delegate()
            {
                if (_currDocType != DocumentType.PayingIn && _currDocType != DocumentType.PayingOut)
                    PrintStringInternal(source, style);
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPrintBarcode({0}, {1}, {2})", barcode, align, readable));
            ExecuteDriverCommand(!IsPrim02, delegate()
            {
                if (IsPrim02)
                {
                    _deviceProtocol.CreateCommand("1A");
                    _deviceProtocol.AddString("02");
                    _deviceProtocol.AddString(readable ? "02" : "00");
                    _deviceProtocol.AddString("00");
                    _deviceProtocol.AddString("46");
                    _deviceProtocol.AddString("02");
                    _deviceProtocol.AddString(barcode);
                    _deviceProtocol.Execute();
                }
                else
                {
                    // расположение
                    switch (align)
                    {
                        case AlignOptions.Left:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 0 });
                            break;
                        case AlignOptions.Center:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 1 });
                            break;
                        case AlignOptions.Right:
                            _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 2 });
                            break;
                    }
                    // высота штрих-кода
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('h'), 70 });
                    // ширина кода
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('w'), 2 });
                    // цифры
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('H'), readable ? (byte)2 : (byte)0 });
                    // тип штрих-кода
                    _deviceProtocol.Write(new byte[] { 0x1D, Convert.ToByte('k'), 0x43, 0x0C });
                    // данные
                    _deviceProtocol.Write(Encoding.ASCII.GetBytes(barcode));
                    // NULL
                    _deviceProtocol.Write(new byte[] { 0x00 });
                    // сброс расположения
                    _deviceProtocol.Write(new byte[] { 0x1B, Convert.ToByte('a'), 0 });
                }
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            _deviceProtocol.WriteDebugLine("OnPrintImage");
            ExecuteDriverCommand(delegate() { });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnRegistration({0}, {1}, {2}, {3})", commentary, quantity, amount, section));
            bool nonFiscalDoc = _currDocType == DocumentType.Other || !HasNonzeroRegistrations;
            ExecuteDriverCommand(nonFiscalDoc && !IsPrim02, delegate()
            {
                // пока выполняется регистрация с нулевой суммой, документ открывать не нужно
                // а то потом с нулевой суммой фискальный чек не закроется
                if (nonFiscalDoc)
                {
                    if (!string.IsNullOrEmpty(commentary))
                        PrintStringInternal(commentary, FontStyle.Regular);
                }
                else
                {
                    // если фискальный документ не открыт, пытаемся его открыть
                    if (_deviceProtocol.GetDeviceInfo().DocState == FiscalDocState.Closed)
                        OpenFiscalDocument();

                    float fPrice = (float)(amount / 100.0);
                    float fQuantity = (float)(quantity / 1000.0);

                    if (fQuantity == 0.0)
                        fQuantity = 1.0f;

                    _deviceProtocol.CreateCommand("11");
                    // название товара
                    commentary = commentary.Length > 40 ? commentary.Substring(0, 40) : commentary;
                    _deviceProtocol.AddString(commentary.Length == 0 ? " " : commentary);
                    // артикул
                    _deviceProtocol.AddString("");
                    // цена
                    _deviceProtocol.AddString(fPrice.ToString("f2", _currNfi));
                    // количество/вес
                    _deviceProtocol.AddString(fQuantity.ToString("f3", _currNfi));
                    // единица измерения
                    _deviceProtocol.AddString(" ");
                    // индекс отдела в ЭКЛЗ
                    _deviceProtocol.AddString(section.ToString("d2"));
                    // идентификатор секции
                    _deviceProtocol.AddString("");
                    _deviceProtocol.Execute();

                    _documentAmount += (uint)Math.Round(amount * quantity / 1000.0);
                }
            });
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnPayment({0}, {1})", amount, paymentType));

            bool nonFiscalDoc = _currDocType == DocumentType.Other || !HasNonzeroRegistrations;
            if (!nonFiscalDoc)
                ExecuteDriverCommand(delegate()
                {
                    // подытог
                    FiscalDocState cuurDocState = _deviceProtocol.GetDeviceInfo().DocState;
                    if (cuurDocState == FiscalDocState.Position)
                        _deviceProtocol.ExecuteCommand("12");

                    decimal fAmount = (decimal)(amount / 100.0);
                    _deviceProtocol.CreateCommand("13");
                    switch (paymentType)
                    {
                        case FiscalPaymentType.Cash:
                            _deviceProtocol.AddString("00");
                            break;
                        case FiscalPaymentType.Card:
                            _deviceProtocol.AddString("02");
                            break;
                        case FiscalPaymentType.Other1:
                            _deviceProtocol.AddString("03");
                            break;
                        case FiscalPaymentType.Other2:
                            _deviceProtocol.AddString("04");
                            break;
                        case FiscalPaymentType.Other3:
                            _deviceProtocol.AddString("05");
                            break;
                    }

                    _deviceProtocol.AddString(fAmount.ToString("f2", _currNfi));
                    _deviceProtocol.AddString("");  // название платежной карты

                    _deviceProtocol.Execute();
                });
        }

        protected override void OnCash(uint amount)
        {
            _deviceProtocol.WriteDebugLine(string.Format("OnCash({0})", amount));
            ExecuteDriverCommand(delegate()
            {
                if (_currDocType == DocumentType.PayingIn || _currDocType == DocumentType.PayingOut)
                    _documentAmount += amount;
            });
        }

        protected override void OnContinuePrint()
        {
            _deviceProtocol.WriteDebugLine("OnContinuePrint");
            base.OnContinuePrint();

            try
            {
                // запрос суммы в кассе
                _docOpened = false;

                // если сумма в кассе не изменилась, значит предыдущий документ не был завершен
                ulong currCashInDrawer = 0;
                int retriesCount = 3;
                do
                {
                    currCashInDrawer = Status.CashInDrawer;
                    if (!ErrorCode.Succeeded && retriesCount == 0)
                        return;
                    retriesCount--;
                }
                while (!ErrorCode.Succeeded);

                _docOpened = currCashInDrawer == _storedCashInDrawer;
                if (_docOpened)
                    _deviceProtocol.WriteDebugLine("Документ не был завершен. Будет произведена повторная печать документа");
                else
                    _deviceProtocol.WriteDebugLine("Документ был успешно завершен");
            }
            finally
            {
                Logger.SaveDebugInfo(this, _deviceProtocol.DebugInfo);
                _deviceProtocol.ClearDebugInfo();
            }
        }

        #endregion

        #region Реализация интерфейса

        #region События

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region Свойства

        public override DateTime CurrentTimestamp
        {
            get
            {
                _deviceProtocol.WriteDebugLine("CurrentTimestamp_get");
                DateTime currTimestamp = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    _deviceProtocol.ExecuteCommand("43");
                    string dateTimeStr = Encoding.ASCII.GetString(_deviceProtocol.GetField(1)) + Encoding.ASCII.GetString(_deviceProtocol.GetField(2));
                    currTimestamp = DateTime.ParseExact(dateTimeStr, "ddMMyyHHmm", null);
                });
                return currTimestamp;
            }
            set
            {
                _deviceProtocol.WriteDebugLine(string.Format("CurrentTimestamp_set({0: yy.MM.dd HH:mm:ss})", value));
                ExecuteDriverCommand(delegate()
                {
                    _deviceProtocol.ExecuteCommand("42", true);
                });
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(MAX_STRING_LEN, 0), true);
            }
        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                _deviceProtocol.WriteDebugLine("Info");
                string SerialNo = "0";
                ExecuteDriverCommand(delegate()
                {
                    SerialNo = _deviceProtocol.GetDeviceInfo().SerialNo;
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypeSpark, SerialNo);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                _deviceProtocol.WriteDebugLine("PrinterStatus");
                bool bPrinting = false;
                PaperOutStatus poStatus = PaperOutStatus.Present;
                bool bDrawerOpened = false;
                byte statusByte = 0;

                // проверяем состояние печатающего устройства
                try
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                    statusByte = _deviceProtocol.ShortStatusRequest(true, 0x30, 5);
                    if ((statusByte & 0x20) == 0)
                        // активное отсутсвие бумаги 
                        return new PrinterStatusFlags(false, PaperOutStatus.OutActive, _docOpened, false);
                    else if (_docOpened)
                        // требуется команда продолжения печати
                        return new PrinterStatusFlags(false, PaperOutStatus.OutAfterActive, _docOpened, false);
                }
                catch (TimeoutException)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
                }

//                return new PrinterStatusFlags(false, PaperOutStatus.Present, _docOpened, false);

                ExecuteDriverCommand(delegate()
                {
                    // байт "Статус печатающего устройства"
                    statusByte = _deviceProtocol.ShortStatusRequest(false, 0x31);
                    bDrawerOpened = (statusByte & 4) == 4;

                    // байт "Состояние датчиков бумаги"
                    statusByte = _deviceProtocol.ShortStatusRequest(false, 0x34);
                    poStatus = ((statusByte & 64) == 64) || ((statusByte & 8) == 8) ? PaperOutStatus.OutPassive : PaperOutStatus.Present;
                });
                return new PrinterStatusFlags(bPrinting, poStatus, _docOpened, bDrawerOpened);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                _deviceProtocol.WriteDebugLine("Status");
                ulong cashInDrawer = 0;
                CurrentDeviceInfo currDevInfo = new CurrentDeviceInfo();

                ExecuteDriverCommand(delegate()
                {
                    currDevInfo = _deviceProtocol.GetDeviceInfo();
                    // сумма в денежном ящике
                    _deviceProtocol.ExecuteCommand("37");
                    string sCash = Encoding.ASCII.GetString(_deviceProtocol.GetField(15));
                    cashInDrawer = Convert.ToUInt64(sCash.Replace(".", ""));
                });

                return new FiscalStatusFlags(currDevInfo.OpenedShift, currDevInfo.OverShift,
                    false, currDevInfo.Fiscalized, (ulong)_documentAmount, (ulong)cashInDrawer);
            }
        }

        #endregion

        #region Методы

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            _deviceProtocol.WriteDebugLine(string.Format("FiscalReport({0}, {1})", reportType, full));
            ExecuteDriverCommand(delegate()
            {
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        _deviceProtocol.CreateCommand(full ? "07" : "05", true);
                        _deviceProtocol.AddString(TaxerPassword.ToString());
                        _deviceProtocol.AddString(((DateTime)reportParams[0]).ToString("ddMMyy"));
                        _deviceProtocol.AddString(((DateTime)reportParams[1]).ToString("ddMMyy"));
                        break;
                    case FiscalReportType.ByShifts:
                        _deviceProtocol.CreateCommand(full ? "08" : "06", true);
                        _deviceProtocol.AddString(TaxerPassword.ToString());
                        _deviceProtocol.AddString(((int)reportParams[0]).ToString("d4"));
                        _deviceProtocol.AddString(((int)reportParams[1]).ToString("d4"));
                        break;
                }
                _deviceProtocol.Execute();
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            _deviceProtocol.WriteDebugLine("Fiscalization");
            ExecuteDriverCommand(delegate()
            {
                _deviceProtocol.CreateCommand("04", true);
                // старый пароль
                _deviceProtocol.AddString(TaxerPassword.ToString());
                // новый пароль
                _deviceProtocol.AddString(newPassword.ToString());
                // новый регистрационный номер
                _deviceProtocol.AddString(registrationNumber.ToString());
                // ИНН
                _deviceProtocol.AddString(taxPayerNumber.ToString());
                // группа ("00" - магазины)
                _deviceProtocol.AddString("00");
                // накопление итога закупок в фискальной памяти ("01" - разрешено)
                _deviceProtocol.AddString("01");

                _deviceProtocol.Execute();
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            _deviceProtocol.WriteDebugLine("GetLifetime");
            firstDate = DateTime.Now;
            lastDate = DateTime.Now;
            firstShift = 1;
            lastShift = 9999;
            ExecuteDriverCommand(delegate() { });
        }

        #endregion

        #endregion

        #region ISparkDeviceProvider Members

        public EasyCommunicationPort GetCommunicationPort()
        {
            return Port;
        }

        public bool GetIsOnlyESCPOSMode()
        {
            return IsSparkOld;
        }

        #endregion
    }

    /// <summary>
    /// Драйвер ФР "Прим", работающих по протоколу последних версий (с командой печати строки). Режим принтера
    /// не используется.
    /// </summary>
    [Serializable]
    [FiscalDevice(DeviceNames.ecrTypeSpark2)]
    public class SparkTK2FiscalDevice : SparkTKFiscalDevice
    {
        protected override bool IsPrim02
        {
            get { return true; }
        }
    }

    /// <summary>
    /// Драйвер ФР "Прим", работающих по протоколу самых старых версий (без команды печати строки). Печать 
    /// произвольных строк производится в режиме принтера. Текущий режим (команды принтера или 
    /// протокольные команды ФР) невозможно получить от устройства. Поэтому текущий режим хранится в 
    /// данных драйвера.
    /// </summary>
    [Serializable]
    [FiscalDevice("Искра 1.1")]
    public class SparkTKOldFiscalDevice : SparkTKFiscalDevice
    {
        protected override bool IsSparkOld
        {
            get { return true; }
        }
    }
}