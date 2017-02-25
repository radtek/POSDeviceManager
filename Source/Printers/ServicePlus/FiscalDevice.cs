using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace ServicePlus
{
    [Serializable]
    [FiscalDevice(DeviceNames.ecrTypeServicePlus)]
    public class FiscalDevice : CustomFiscalDevice
    {
        #region Константы

        private const int MAX_STRING_LEN = 40;
        private const int READ_TIMEOUT = 5000;
        private const int WRITE_TIMEOUT = 1000;
        private string PASSWORD = "PONE";

        #endregion

        #region Поля

        private SPProtocol _spProtocol;

        private DocumentType _currDocType = DocumentType.Other;

        private PaperOutStatus _paperStatus = PaperOutStatus.Present;

        #endregion

        #region События

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region Свойства

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
                string serial = String.Empty;
                ExecuteDriverCommand(false, protocol =>
                {
                    protocol.ExecuteCommand("A6");
                    serial = protocol.GetFieldAsString(1);
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypeServicePlus, serial);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                byte currStatus = 0;
                ulong cashAmount = 0;
                ulong docAmount = 0;
                ExecuteDriverCommand(false, protocol =>
                {
                    // флаги статуса ФР
                    protocol.ExecuteCommand("A0");
                    currStatus = (byte)protocol.GetFieldAsInt(2);

                    // состояние смены: 5 - сумма наличных в ДЯ
                    protocol.ExecuteCommand("A5", "5");
                    cashAmount = Convert.ToUInt64(protocol.GetFieldAsDecimal(1) * 100m);

                    // состояние смены: 12 - счетчики документа
                    protocol.ExecuteCommand("A5", "12");
                    docAmount = Convert.ToUInt64(protocol.GetFieldAsDecimal(1) * 100m);
                });
                bool openedShift = (currStatus & 0x04) == 0x04;
                bool overShift = (currStatus & 0x08) == 0x08;
                bool fiscalized = (currStatus & 0x02) == 0x00;
                return new FiscalStatusFlags(openedShift, overShift, false, fiscalized, docAmount, cashAmount);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {            
            get 
            {
                bool printing = false;
                bool docOpened = false;
                bool drawerOpened = false;
                
                // состояние бумаги
                if (_paperStatus != PaperOutStatus.Present)
                {
                    var currTime = DateTime.Now;
                    ExecuteDriverCommand("00", true, currTime.ToString("ddMMyy"), currTime.ToString("HHmmss"));
                }

                ExecuteDriverCommand(false, protocol =>
                {
                    // короткий запрос состояния
                    printing = !protocol.ShortStatusInquiry();

                    // флаги статуса ФР
                    protocol.ExecuteCommand("A0");
                    byte docStatus = (byte)protocol.GetFieldAsInt(3);
                    docOpened = (docStatus & 0x0F) != 0;

                    // состояние денежного ящика
                    protocol.ExecuteCommand("A8");
                    drawerOpened = protocol.GetFieldAsInt(1) == 1;
                });
                return new PrinterStatusFlags(printing, _paperStatus, docOpened, drawerOpened);
            }
        }

        public override DateTime CurrentTimestamp
        {
            get
            {
                DateTime currTime = DateTime.Now;
                ExecuteDriverCommand(false, protocol =>
                {
                    protocol.ExecuteCommand("A7");
                    currTime = DateTime.ParseExact(protocol.GetFieldAsString(1) + protocol.GetFieldAsString(2),
                        "ddMMyyHHmmss", null);
                });
                return currTime;
            }
            set
            {
                var currTime = DateTime.Now;
                ExecuteDriverCommand("A3", false, currTime.ToString("ddMMyy"), currTime.ToString("HHmmss"));
            }
        }

        #endregion

        #region Делегаты

        private delegate void ExecuteCommandDelegate(SPProtocol protocol);

        #endregion

        #region Конструктор

        public FiscalDevice()
            : base()
        {
            AddSpecificError(0, "Команда выполнена без ошибок");
            AddSpecificError(1, "Функция невыполнима при данном статусе ККМ");
            AddSpecificError(2, "В команде указан неверный номер функции");
            AddSpecificError(3, "В команде указано неверное, больше чем максимально возможное или несоответствующее типу данных значение");
            AddSpecificError(4, "Переполнение буфера коммуникационного порта");
            AddSpecificError(5, "Таймаут при передаче байта информации");
            AddSpecificError(6, "В команде указан неверный пароль");
            AddSpecificError(7, "Ошибка контрольной суммы в команде");
            AddSpecificError(8, "Конец бумаги");
            AddSpecificError(9, "Принтер не готов");
            AddSpecificError(10, "Текущая смена больше 24 часов");
            AddSpecificError(11, "Разница во времени, ККМ и указанной в команде установки времени, больше 8 минут");
            AddSpecificError(12, "Время последнего документа больше нового времени более чем на один час (с учетом летнего/зимнего перехода)");
            AddSpecificError(13, "Не был задан заголовок документа, что делает невозможным формирование фискального документа.");
            AddSpecificError(14, "Отрицательный результат");
            AddSpecificError(15, "Дисплей покупателя не готов");
            AddSpecificError(32, "Фатальная ошибка ККМ");
            AddSpecificError(33, "Нет свободного места в фискальной памяти ККМ");
            AddSpecificError(65, "Некорректный формат или параметр команды");
            AddSpecificError(66, "Некорректное состояние ЭКЛЗ");
            AddSpecificError(67, "Авария ЭКЛЗ");
            AddSpecificError(68, "Авария КС (Криптографического сопроцессора) в составе ЭКЛЗ");
            AddSpecificError(69, "Исчерпан временной ресурс использования ЭКЛЗ");
            AddSpecificError(70, "ЭКЛЗ переполнена");
            AddSpecificError(71, "Неверные дата или время");
            AddSpecificError(72, "Нет запрошенных данных");
            AddSpecificError(73, "Переполнение (отрицательный итог документа, слишком много отделов для клиента)");
            AddSpecificError(74, "Нет ответа от ЭКЛЗ");
            AddSpecificError(75, "Ошибка при обмене данными с ЭКЛЗ");
        }

        #endregion

        #region Скрытые методы

        private void ExecuteDriverCommand(bool printable, ExecuteCommandDelegate executeCommandDelegate)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);

            if (!Active)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                return;
            }

            try
            {
                if (_spProtocol == null)
                    _spProtocol = new SPProtocol(Port, PASSWORD);
                executeCommandDelegate(_spProtocol);
                if (printable)
                    _paperStatus = PaperOutStatus.Present;
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _spProtocol.GetCommandDump());
            }
            catch (DeviceErrorException E)
            {
                // отлавливаем ошибки принтера
                if (E.ErrorCode == 8 || E.ErrorCode == 9)
                    _paperStatus = PaperOutStatus.OutPassive;
                else if (E.ErrorCode == 1 && _paperStatus != PaperOutStatus.Present)
                {
                }

                ErrorCode = new ServerErrorCode(this, E.ErrorCode, GetSpecificDescription(E.ErrorCode),
                    _spProtocol.GetCommandDump());
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
        }

        private void ExecuteDriverCommand(string code, bool printable, params string[] args)
        {
            ExecuteDriverCommand(printable, protocol => protocol.ExecuteCommand(code, args));
        }

        private void ExecuteDriverCommand(string code, params string[] args)
        {
            ExecuteDriverCommand(false, protocol => protocol.ExecuteCommand(code, args));
        }

        #endregion

        #region Методы

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            base.SetCommStateEventHandler(sender, e);
        }

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            _spProtocol = new SPProtocol(Port, PASSWORD);

            try
            {
                // короткий запрос состояния
                _spProtocol.ShortStatusInquiry();

                // проверяем статус
                _spProtocol.ExecuteCommand("A0");
                byte currStatus = (byte)_spProtocol.GetFieldAsInt(2);

                // если команда инициализации еще не выполнялась
                if ((currStatus & 0x01) == 0x01)
                {
                    // инициализация ККМ
                    var currTime = DateTime.Now;
                    _spProtocol.ExecuteCommand("00", currTime.ToString("ddMMyy"), currTime.ToString("HHmmss"));

                }
                // если смена закрыта
                if ((currStatus & 0x04) == 0x00)
                {
                    // установка клише
                    for (int i = 0; i < DocumentHeader.Length && i < 4; i++)
                        _spProtocol.ExecuteCommand("A2", "20", i.ToString(), DocumentHeader[i]);

                    // установка подвала
                    for (int i = 0; i < DocumentFooter.Length && i < 2; i++)
                        _spProtocol.ExecuteCommand("A2", "21", i.ToString(), DocumentFooter[i]);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected override void OnOpenDocument(DocumentType docType, string cashierName)
        {
            ExecuteDriverCommand(true, protocol =>
            {
                _currDocType = docType;

                // проверка состояния документа
                protocol.ExecuteCommand("A0");
                byte docStatus = (byte)protocol.GetFieldAsInt(3);
                if ((docStatus & 0x0F) != 0)
                    // отмена документа
                    protocol.ExecuteCommand("23");

                switch (docType)
                {
                    case DocumentType.Sale:
                        // открываем документ
                        protocol.ExecuteCommand("20",
                            "2", // тип документа
                            "1", // номер отдела
                            cashierName, // имя оператора
                            String.Empty); // номер документа
                        break;
                    case DocumentType.Refund:
                        protocol.ExecuteCommand("20", "3", "1", cashierName, String.Empty);
                        break;
                    case DocumentType.Other:
                        protocol.ExecuteCommand("20", "1", "1", cashierName, String.Empty);
                        break;

                    case DocumentType.PayingIn:
                        protocol.ExecuteCommand("20", "4", "1", cashierName, String.Empty);
                        break;
                    case DocumentType.PayingOut:
                        protocol.ExecuteCommand("20", "5", "1", cashierName, String.Empty);
                        break;

                    case DocumentType.SectionsReport:
                    case DocumentType.XReport:
                        protocol.ExecuteCommand("60", true, cashierName);
                        break;
                    case DocumentType.ZReport:
                        protocol.ExecuteCommand("61", true, cashierName);
                        break;
                }
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            byte styleByte = 0;
            switch (style)
            {
                case FontStyle.DoubleAll:
                    styleByte = 0x30;
                    break;
                case FontStyle.DoubleHeight:
                    styleByte = 0x10;
                    break;
                case FontStyle.DoubleWidth:
                    styleByte = 0x20;
                    break;
            }

            ExecuteDriverCommand("21", true, source, styleByte.ToString());
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount, byte section)
        {
            if (_currDocType == DocumentType.Sale ||
                _currDocType == DocumentType.Refund)
                ExecuteDriverCommand("30", true, 
                commentary,
                String.Empty,
                (quantity / 1000m).ToString("F3", SPProtocol.Nfi),
                (10m * amount / quantity).ToString("F2", SPProtocol.Nfi),
                section.ToString(),
                String.Empty);
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            if (_currDocType == DocumentType.Sale ||
                _currDocType == DocumentType.Refund)
                ExecuteDriverCommand(true, protocol =>
                {
                    // проверка состояния документа
                    protocol.ExecuteCommand("A0");
                    byte docStatus = (byte)protocol.GetFieldAsInt(3);
                    // если подытог еще не подводился
                    if ((docStatus & 0xF0) == 0x10)
                        // выполняем команду "Подытог"
                        protocol.ExecuteCommand("34");

                    // выполняем команду "Платеж"
                    protocol.ExecuteCommand("35", Convert.ToString((int)paymentType),
                        (amount / 100m).ToString("F2", SPProtocol.Nfi));
                });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            if (_currDocType != DocumentType.XReport &&
                _currDocType != DocumentType.ZReport &&
                _currDocType != DocumentType.SectionsReport)
                ExecuteDriverCommand("22");

            // проверяем статус документа
            if (ErrorCode.Succeeded && _paperStatus != PaperOutStatus.Present)
                ExecuteDriverCommand(true, protocol =>
                {
                    protocol.ExecuteCommand("A0");
                    int status = protocol.GetFieldAsInt(3);
                    status >>= 4;
                    if (status == 8)
                        // Команда закрытия была отправлена в ЭКЛЗ, но документ не был завершен
                        // Требуется команда завершения печати
                        _paperStatus = PaperOutStatus.OutActive;
                });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand("36", true, String.Empty, (amount / 100m).ToString("F2", SPProtocol.Nfi));
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align, bool readable)
        {
            ExecuteDriverCommand("24", true, readable ? "2" : "0", "2", "70", "2", barcode);
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand("80");
            System.Threading.Thread.Sleep(500);
        }

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            switch (reportType)
            {
                case FiscalReportType.ByDates:
                    ExecuteDriverCommand("63", true, full ? "1" : "0",
                        ((DateTime)reportParams[0]).ToString("ddMMyy"),
                        ((DateTime)reportParams[1]).ToString("ddMMyy"),
                        TaxerPassword.ToString());
                    break;
                case FiscalReportType.ByShifts:
                    ExecuteDriverCommand("62", true, full ? "1" : "0",
                        ((int)reportParams[0]).ToString(),
                        ((int)reportParams[1]).ToString(),
                        TaxerPassword.ToString());
                    break;
            }
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            ExecuteDriverCommand("08", true, TaxerPassword.ToString(), registrationNumber.ToString(),
                taxPayerNumber.ToString(), newPassword.ToString());
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, 
            out int firstShift, out int lastShift)
        {
            int firstShiftInt = 0;
            int lastShiftInt = 9999;

            ExecuteDriverCommand(false, protocol =>
            {
                protocol.ExecuteCommand("A5", "2");
                firstShiftInt = _spProtocol.GetFieldAsInt(1);

                protocol.ExecuteCommand("A5", "1");
                lastShiftInt = _spProtocol.GetFieldAsInt(1);
            });

            firstShift = firstShiftInt;
            lastShift = lastShiftInt;

            firstDate = new DateTime(1900, 1, 1);
            lastDate = DateTime.Now;
        }


        protected override void OnContinuePrint()
        {
            // повторное закрытие документа
            OnCloseDocument(true);
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(true, protocol => { });
        }
        
        #endregion
    }
}
