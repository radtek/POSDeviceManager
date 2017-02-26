using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace POSPrint
{
    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypePilot)]
    public class POSPrintFiscalDevice : CustomFiscalDevice
    {
        #region Константы

        // максимальная длина строки
        private const int MAX_STRING_LEN = 48;

        private const int READ_TIMEOUT = 500;
        private const int WRITE_TIMEOUT = 500;

        // пароль оператора
        private const string OPERATOR_PASSWD = "0000";

        #endregion

        #region Внутренние переменные

        // код ошибки
        private short _errorCode = 0;

        // текущий тип документа
        DocumentType m_CurrentDocType = DocumentType.Other;

        #endregion

        #region Конструктор

        public POSPrintFiscalDevice()
            : base()
        {
            AddSpecificError(1, "Неизвестная команда. Задан неверный код команды");
            AddSpecificError(2, "Команда запрещена в данном режиме ФП. Попытка выполнить команду которая не допустима при данном положении переключателей ККМ");
            AddSpecificError(3, "Команда запрещена в данном состоянии ФП");
            AddSpecificError(4, "Ошибка записи в ПЗУ ФП");
            AddSpecificError(5, "Ошибка записи в NVR ФП");
            AddSpecificError(6, "Ошибка чтения ПЗУ ФП");
            AddSpecificError(7, "Ошибка чтения NVR ФП");
            AddSpecificError(8, "Ошибка принтера");
            AddSpecificError(12, "Неверные параметры команды. Формат параметров принятой команды отличается от ожидаемого формата");
            AddSpecificError(13, "Недостаточно памяти для закрытия смены. Исчерпан лимит закрытых смен");
            AddSpecificError(14, "Неверный формат команды. Количество параметров в пакете команды отличается от ожидаемого");
            AddSpecificError(15, "Неверный пароль. Пароль переданый в команде отличается от установленного в ККМ");
            AddSpecificError(16, "Не достаточно памяти для фискализации. Исчерпан лимит записей для перерегистраций ККМ или активизации ЭКЛЗ");
            AddSpecificError(17, "Переполнение дневного итога. В результате попытки выполнить команду переполнился соответствующий регистр ККМ");
            AddSpecificError(18, "Переполнение итога документа. В результате попытки выполнить команду переполнен соответствующий регистр документа");
            AddSpecificError(19, "Превышена продолжительность смены");
            AddSpecificError(21, "Ошибка часов ФП");
            AddSpecificError(22, "Ошибка, если подтверждающая дата не совпадает с первоначальной для команды SETTIMER");
            AddSpecificError(23, "Код возврата после первой команды SETTIMER. Означает что ККМ ждет повторной команды SETTIMER с теми же самыми параметрами что были использованы для предыдущей команды");
            AddSpecificError(24, "Фискальный буфер переполнен. Означает что требуется выдать на печать накопленный в ККМ буфер с помощью команды FLUSHBUFFER");
            AddSpecificError(25, "Невозможно выполнить команду REPEAT – нет невыполненной команды");
            AddSpecificError(26, "Нет возможности завершить команду (вероятнее из-за принтера)");
            AddSpecificError(27, "Нет места для смен");
            AddSpecificError(28, "Нет места для записи активизации ЭКЛЗ");
            AddSpecificError(29, "Команда не применима к текущему (открытому) документу");
            AddSpecificError(30, "ФП не фискализована");
            AddSpecificError(31, "Не установлен заводской номер ККМ");
            AddSpecificError(34, "Документ не открыт");
            AddSpecificError(35, "Смена не открыта");
            AddSpecificError(36, "Клише не установлено");
            AddSpecificError(37, "Часы ККМ не установлены");
            AddSpecificError(38, "Команда не завершена");
            AddSpecificError(39, "В ФП нет места для записи смен");
            AddSpecificError(40, "Логическая ошибка в ФП");
            AddSpecificError(41, "Аппаратная ошибка ЭКЛЗ");
            AddSpecificError(42, "ФП фискализована");
            AddSpecificError(43, "Заводской номер уже установлен");
            AddSpecificError(45, "ККМ блокирована");
            AddSpecificError(46, "Документ открыт");
            AddSpecificError(47, "Смена открыта");
            AddSpecificError(81, "Ошибка в параметрах ЭКЛЗ");
            AddSpecificError(82, "Ошибочное состояние ЭКЛЗ");
            AddSpecificError(83, "Аварийное состояние ЭКЛЗ");
            AddSpecificError(84, "Авария криптопроцессора ЭКЛЗ");
            AddSpecificError(85, "Исчерпан временной лимит ЗКЛЗ");
            AddSpecificError(86, "ЭКЛЗ переполнена");
            AddSpecificError(87, "Неверные дата/время в параметрах ЭКЛЗ");
            AddSpecificError(88, "Нет данных для отчета");
            AddSpecificError(89, "Переполнение регистров ЭКЛЗ");
            AddSpecificError(91, "ЭКЛЗ не активизирована");
            AddSpecificError(92, "Архив ЭКЛЗ закрыт");
            AddSpecificError(93, "ЭКЛЗ активизирована не на этой ККМ");
            AddSpecificError(94, "ЭКЛЗ уже активизирована");
            AddSpecificError(95, "Исчерпан ресурс ЭКЛЗ (временной или свободное место)");
            AddSpecificError(99, "ЭКЛЗ offline");
        }

        #endregion

        #region Внутренние методы

        private delegate void ExecuteCommandDelegate();

        private void ExecuteDriverCommand(ExecuteCommandDelegate executeCommandDelegate)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            try
            {
                if (!Active)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                    return;
                }

                executeCommandDelegate();

                if (_errorCode < 0x1000)
                    ErrorCode = new ServerErrorCode(this, _errorCode, GetSpecificDescription(_errorCode));
                else
                    ErrorCode = new ServerErrorCode(this, (GeneralError)_errorCode);
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout);
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
            }
        }

        // печать отчетов
        private short PrintReport(DocumentType docType)
        {
            POSCommand Cmd = null;

            switch (docType)
            {
                case DocumentType.XReport:
                case DocumentType.SectionsReport:
                    Cmd = new POSCommand(218);
                    Cmd.AddChar(OPERATOR_PASSWD, 4);
                    break;

                case DocumentType.ZReport:
                    Cmd = new POSCommand(204);
                    Cmd.AddChar(OPERATOR_PASSWD, 4);
                    Cmd.AddNumeric(1, 2);
                    break;
            }

            return Cmd.Execute(Port);
        }

        // отмена документа
        private short CancelDocument()
        {
            POSCommand Cmd = new POSCommand(216);   // Отменить документ
            Cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль

            return Cmd.Execute(Port);
        }

        // продолжение печати
        private bool ContinuePrint()
        {
            POSCommand Cmd = new POSCommand(405);   // повтор команды
            Cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль
            _errorCode = Cmd.Execute(Port);
            return _errorCode == (short)GeneralError.Success;
        }

        // установка клише документа
        private bool SetDocumentHeader()
        {
            if (DocumentHeader == null)
                return true;

            POSCommand Cmd = null;

            // очистка клише
            Cmd = new POSCommand(201);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            _errorCode = Cmd.Execute(Port);
            if (_errorCode != (short)GeneralError.Success)
                return false;

            // установка строк клише
            for (int i = 0; i < DocumentHeader.Length; i++)
            {
                Cmd = new POSCommand(202);   // установить строку клише
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(i + 1, 2);
                Cmd.AddNumeric(1, 2);
                if (DocumentHeader[i].Length > MAX_STRING_LEN)
                    Cmd.AddBChar(DocumentHeader[i].Substring(0, MAX_STRING_LEN));
                else
                    Cmd.AddBChar(DocumentHeader[i]);
                _errorCode = Cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    break;
            }
            return _errorCode == (short)GeneralError.Success;
        }

        // установка подвала документа
        private bool SetDocumentFooter()
        {
            if (DocumentFooter == null)
                return true;

            POSCommand Cmd = null;

            // очистка подвала
            Cmd = new POSCommand(233);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            _errorCode = Cmd.Execute(Port);
            if (_errorCode != (short)GeneralError.Success)
                return false;

            // установка строк подвала
            for (int i = 0; i < DocumentFooter.Length; i++)
            {
                Cmd = new POSCommand(234);   // установить строку подвала
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(i + 1, 2);
                Cmd.AddNumeric(1, 2);
                if (DocumentFooter[i].Length > MAX_STRING_LEN)
                    Cmd.AddBChar(DocumentFooter[i].Substring(0, MAX_STRING_LEN));
                else
                    Cmd.AddBChar(DocumentFooter[i]);
                _errorCode = Cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    break;
            }

            return _errorCode == (short)GeneralError.Success;
        }

        // установка сообщений
        private bool SetConst()
        {
            POSCommand Cmd = new POSCommand(231);
            Cmd.AddChar(OPERATOR_PASSWD, 4);
            Cmd.AddNumeric(5, 2);
            Cmd.AddBChar("НЕФИСКАЛЬНЫЙ ДОКУМЕНТ");
            _errorCode = Cmd.Execute(Port);
            return _errorCode == (short)GeneralError.Success;
        }

        #endregion

        #region Реализация виртуальных функций

        protected override void OnAfterActivate()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                Port.ReadTimeout = READ_TIMEOUT;
                Port.WriteTimeout = WRITE_TIMEOUT;

                if (!SetConst())
                    return;

                // проверка состояния бумаги
                if (PrinterStatus.PaperOut == PaperOutStatus.OutAfterActive)
                    if (!ContinuePrint())
                        return;

                if (!SetDocumentHeader())
                    return;

                if (!SetDocumentFooter())
                    return;
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            string cashierName)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                if (PrinterStatus.OpenedDocument)
                    _errorCode = CancelDocument();

                m_CurrentDocType = docType;
                POSCommand cmd = new POSCommand(205);
                cmd.AddChar(OPERATOR_PASSWD, 4);
                switch (docType)
                {
                    case DocumentType.Sale:
                        cmd.AddNumeric(0, 2);
                        break;
                    case DocumentType.Refund:
                        cmd.AddNumeric(1, 2);
                        break;
                    case DocumentType.PayingIn:
                        cmd.AddNumeric(2, 2);
                        break;
                    case DocumentType.PayingOut:
                        cmd.AddNumeric(3, 2);
                        break;
                    case DocumentType.Other:
                        cmd.AddNumeric(4, 2);
                        break;
                    case DocumentType.XReport:
                    case DocumentType.ZReport:
                    case DocumentType.SectionsReport:
                        //                        m_nErrorCode = PrintReport(docType);
                        return;
                }

                cmd.AddBChar(cashierName);  // имя оператора
                cmd.AddNumeric(13, 2);      // код оператора
                cmd.AddBChar("");           // отдел
                cmd.AddBChar("");           // подотдел

                byte[] nRsp = new byte[5];
                _errorCode = cmd.Execute(Port, nRsp);
            });
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                switch (m_CurrentDocType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                    case DocumentType.PayingIn:
                    case DocumentType.PayingOut:
                    case DocumentType.Other:
                        POSCommand cmd = new POSCommand(215);   // закрыть документ
                        cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль
                        cmd.AddNumeric(Convert.ToInt32(cutPaper), 1); // обрезать чек
                        _errorCode = cmd.Execute(Port);
                        if (_errorCode != (short)GeneralError.Success)
                            CancelDocument();
                        break;
                    case DocumentType.XReport:
                    case DocumentType.SectionsReport:
                    case DocumentType.ZReport:
                        _errorCode = PrintReport(m_CurrentDocType);
                        break;
                }
            });
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(236);
                cmd.AddChar(OPERATOR_PASSWD, 4);
                cmd.AddNumeric(0, 1);
                _errorCode = cmd.Execute(Port);
            });
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                if (m_CurrentDocType == DocumentType.XReport
                    || m_CurrentDocType == DocumentType.ZReport
                    || m_CurrentDocType == DocumentType.SectionsReport)
                {
                    return;
                }

                POSCommand cmd = new POSCommand(206);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль
                cmd.AddNumeric(0, 1);               // устройство
                cmd.AddBChar(source);  // текст

                _errorCode = cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();

            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                byte[] nPrintBuffer = new byte[256];
                int BufSize = 0;

                // позиционирование по центру
                nPrintBuffer[BufSize++] = 0x1B;
                nPrintBuffer[BufSize++] = Convert.ToByte('a');
                nPrintBuffer[BufSize++] = 1;

                // высота штрих-кода
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('h');
                nPrintBuffer[BufSize++] = 70;

                // ширина кода
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('w');
                nPrintBuffer[BufSize++] = 2;

                // цифры
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('H');
                if (readable)
                    nPrintBuffer[BufSize++] = 2;
                else
                    nPrintBuffer[BufSize++] = 0;

                // штрих-код
                nPrintBuffer[BufSize++] = 0x1D;
                nPrintBuffer[BufSize++] = Convert.ToByte('k');
                nPrintBuffer[BufSize++] = 0x43;
                nPrintBuffer[BufSize++] = 0x0C;

                POSCommand cmd = new POSCommand(237);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль
                cmd.AddNumeric(0, 1);               // устройство
                cmd.AddBChar(Encoding.ASCII.GetString(nPrintBuffer, 0, BufSize) + barcode);          // данные

                _errorCode = cmd.Execute(Port);

                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate() { });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount,
            byte section)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(209);
                cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль
                cmd.AddNumeric(section, 3);       // номер отдела
                cmd.AddBChar("");                   // штрих-код
                cmd.AddBChar("");                   // внутренний учетный код
                cmd.AddBChar(commentary);         // название
                cmd.AddNumeric((int)amount, 10);    // цена
                cmd.AddNumeric((int)quantity, 8);// количество
                cmd.AddNumeric(0, 10);                // стоимость тары
                cmd.AddBChar("");                   // размерность

                byte[] nRsp = new byte[5];

                _errorCode = cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();

            });
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand cmd = new POSCommand(214);
                cmd.AddChar(OPERATOR_PASSWD, 4);

                switch (paymentType)
                {
                    case FiscalPaymentType.Cash:
                        cmd.AddNumeric(0, 2);
                        break;
                    case FiscalPaymentType.Card:
                        cmd.AddNumeric(2, 2);
                        break;
                    default:
                        cmd.AddNumeric(3, 2);
                        break;
                }
                cmd.AddNumeric((int)amount, 10);

                byte[] nRsp = new byte[11];
                _errorCode = cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand Cmd = new POSCommand(214);   // внести сумму
                Cmd.AddChar(OPERATOR_PASSWD, 4);        // пароль
                Cmd.AddNumeric(0, 2);                   // тип оплаты
                Cmd.AddNumeric((int)amount, 10);        // сумма

                byte[] nRsp = new byte[11];
                _errorCode = Cmd.Execute(Port, nRsp);
                if (_errorCode != (short)GeneralError.Success)
                    CancelDocument();
            });
        }

        protected override void OnContinuePrint()
        {
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                ContinuePrint();
            });
        }

        #endregion

        #region Реализация интерфейса

        #region События

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        #endregion

        #region Свойства

        public override DateTime CurrentTimestamp
        {
            get
            {
                DateTime currDateTime = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand cmd = new POSCommand(104);
                    cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль

                    byte[] Rsp = new byte[14];
                    _errorCode = cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        currDateTime = new DateTime(
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4, 4)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 2, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 8, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 10, 2)),
                            Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 12, 2))
                            );
                });
                return currDateTime;
            }
            set
            {
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    DateTime dtValue = value;

                    POSCommand cmd = new POSCommand(401);
                    cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль
                    cmd.AddNumeric(dtValue.Day, 2);         // день
                    cmd.AddNumeric(dtValue.Month, 2);       // месяц
                    cmd.AddNumeric(dtValue.Year, 4);        // год
                    cmd.AddNumeric(dtValue.Hour, 2);        // час
                    cmd.AddNumeric(dtValue.Minute, 2);      // минута
                    cmd.AddNumeric(dtValue.Second, 2);      // секунда

                    _errorCode = cmd.Execute(Port);
                });
            }

        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                string serialNo = "";
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(103);          // получить заводской номер
                    Cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль
                    byte[] Rsp = new byte[12];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        serialNo = Encoding.ASCII.GetString(Rsp, 0, 12);
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypePilot, serialNo);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                bool bOverShift = false;
                bool bOpenedShift = false;
                bool bLocked = false;
                bool bFiscalized = false;
                int nCashInDrawer = 0;
                int nDocAmount = 0;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(101);   // получить статус
                    Cmd.AddChar(OPERATOR_PASSWD, 4);        // пароль
                    Cmd.AddNumeric(1, 1);                   // с опросом принтера
                    byte[] Rsp = new byte[4];

                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                    {
                        bOpenedShift = ((Rsp[0] & 0x10) == 0x10);
                        bOverShift = ((Rsp[1] & 0x2) == 0x2);
                        bLocked = ((Rsp[1] & 0x1) == 0x1);
                        bFiscalized = ((Rsp[0] & 0x4) == 0x4);
                    }

                    // сумма в ДЯ
                    Cmd = new POSCommand(111);   // получить значение регистра
                    Cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль
                    Cmd.AddNumeric(23, 3);                  // номер регистра (сумма в денежном ящике)

                    Rsp = new byte[16];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        nCashInDrawer = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 16));

                    // сумма документа
                    bool IsOpenAndFiscal = false;
                    Rsp = new byte[33];

                    Cmd = new POSCommand(110);   // получить данные об открытом документе
                    Cmd.AddChar(OPERATOR_PASSWD, 4);       // пароль
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        IsOpenAndFiscal = (Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 1)) > 0) && (Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 1, 2)) != 4);

                    if (IsOpenAndFiscal)
                    {
                        Cmd = new POSCommand(111);          // получить значение регистра
                        Cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль
                        Cmd.AddNumeric(105, 3);             // номер регистра (общий регистр продаж в чеке)
                        Rsp = new byte[16];
                        _errorCode = Cmd.Execute(Port, Rsp);
                        if (_errorCode == (short)GeneralError.Success)
                            nDocAmount = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 16));
                    }
                });
                return new FiscalStatusFlags(bOpenedShift, bOverShift, bLocked, bFiscalized, (ulong)nDocAmount, (ulong)nCashInDrawer);
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(MAX_STRING_LEN, 0), false);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                bool bPrinting = false;
                PaperOutStatus poStatus = PaperOutStatus.OutPassive;
                bool bDrawerOpened = false;
                bool bDocOpened = false;
                ExecuteDriverCommand(delegate()
                {
                    _errorCode = (short)GeneralError.Success;
                    POSCommand Cmd = new POSCommand(101);   // получить статус
                    Cmd.AddChar(OPERATOR_PASSWD, 4);        // пароль
                    Cmd.AddNumeric(1, 1);                   // с опросом принтера
                    byte[] Rsp = new byte[4];

                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                    {
                        bDocOpened = ((Rsp[0] & 0x20) == 0x20);
                        bPrinting = ((Rsp[3] & 0x2) == 0x2);
                        bool bLocked = ((Rsp[1] & 0x1) == 0x1);
                        bool bPaperOut = ((Rsp[3] & 0x4) == 0x4);

                        if (bLocked && bPaperOut)
                            poStatus = PaperOutStatus.OutActive;
                        else if (bPaperOut)
                            poStatus = PaperOutStatus.OutPassive;
                        else if (bLocked)
                            poStatus = PaperOutStatus.OutAfterActive;
                        else
                            poStatus = PaperOutStatus.Present;
                    }

                    // состояние ДЯ
                    Cmd = new POSCommand(128);          // получить состояние денежного ящика
                    Cmd.AddChar(OPERATOR_PASSWD, 4);   // пароль

                    Rsp = new byte[1];
                    _errorCode = Cmd.Execute(Port, Rsp);
                    if (_errorCode == (short)GeneralError.Success)
                        bDrawerOpened = Rsp[0] == '1';
                });
                return new PrinterStatusFlags(bPrinting, poStatus, bDocOpened, bDrawerOpened);
            }
        }

        #endregion

        #region Методы налогового инспектора

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            ExecuteDriverCommand(delegate()
            {
                POSCommand Cmd = null;
                _errorCode = (short)GeneralError.Success;
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        DateTime firstDate = (DateTime)reportParams[0];
                        DateTime lastDate = (DateTime)reportParams[1];

                        Cmd = new POSCommand(303);      // фискальный отчет по датам
                        Cmd.AddNumeric(TaxerPassword, 8);    // пароль
                        Cmd.AddNumeric(firstDate.Day, 2);  // день 
                        Cmd.AddNumeric(firstDate.Month, 2); // месяц
                        Cmd.AddNumeric(firstDate.Year, 4);  // год
                        Cmd.AddNumeric(lastDate.Day, 2);   // день
                        Cmd.AddNumeric(lastDate.Month, 2); // месяц
                        Cmd.AddNumeric(lastDate.Year, 4);  // год
                        Cmd.AddNumeric(Convert.ToInt32(full), 1);        // полный или краткий отчет
                        break;
                    case FiscalReportType.ByShifts:
                        Cmd = new POSCommand(304);      // фискальный отчет по сменам
                        Cmd.AddNumeric(TaxerPassword, 8);    // пароль
                        Cmd.AddNumeric((int)(reportParams[0]), 4);  // начальная смена
                        Cmd.AddNumeric((int)(reportParams[1]), 4);  // конечная смена
                        Cmd.AddNumeric(Convert.ToInt32(full), 1);        // полный или краткий отчет
                        break;
                }

                _errorCode = Cmd.Execute(Port);
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            ExecuteDriverCommand(delegate()
            {
                string sCurrentPasswd;
                _errorCode = (short)GeneralError.Success;
                if (!this.Status.Fiscalized)
                {
                    StringBuilder sBuilder = new StringBuilder(8);
                    sBuilder.Insert(0, " ", 8);
                    sBuilder.Insert(0, this.Info.SerialNo);
                    sCurrentPasswd = sBuilder.ToString(0, 8);
                }
                else
                {
                    sCurrentPasswd = TaxerPassword.ToString("d8");
                }

                POSCommand Cmd = new POSCommand(302);
                Cmd.AddChar(sCurrentPasswd, 8);
                Cmd.AddNumeric((int)registrationNumber, 12);
                Cmd.AddNumeric((int)taxPayerNumber, 12);
                Cmd.AddNumeric(2, 1);
                Cmd.AddNumeric(newPassword, 8);

                _errorCode = Cmd.Execute(Port);
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            firstDate = new DateTime();
            lastDate = new DateTime();
            firstShift = 0;
            lastShift = 0;

            DateTime _firstDate = firstDate;
            DateTime _lastDate = lastDate;
            int _firstShift = firstShift;
            int _lastShift = lastShift;
            ExecuteDriverCommand(delegate()
            {
                _errorCode = (short)GeneralError.Success;

                POSCommand Cmd = new POSCommand(106);   // получить номер последней закрытой смены
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                byte[] Rsp = new byte[4];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _firstShift = 1;
                _lastShift = Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 0, 4));

                Cmd = new POSCommand(109);              // получить данные о первой закрытой смене
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(_firstShift, 4);

                Rsp = new byte[24];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _firstDate = new DateTime(
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 4, 4)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 2, 2)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 0, 2))
                    );

                Cmd = new POSCommand(109);              // получить данные о последней закрытой смене
                Cmd.AddChar(OPERATOR_PASSWD, 4);
                Cmd.AddNumeric(_lastShift, 4);

                Rsp = new byte[24];
                _errorCode = Cmd.Execute(Port, Rsp);
                if (_errorCode != (short)GeneralError.Success)
                    return;

                _lastDate = new DateTime(
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 4, 4)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 2, 2)),
                    Convert.ToInt32(Encoding.ASCII.GetString(Rsp, 4 + 0, 2)));
            });

            firstDate = _firstDate;
            lastDate = _lastDate;
            firstShift = _firstShift;
            lastShift = _lastShift;
        }

        #endregion

        #endregion
    }
}
