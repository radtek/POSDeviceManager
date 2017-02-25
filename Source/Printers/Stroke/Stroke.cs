using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using System.Drawing.Imaging;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Stroke
{
    internal enum StrokeType { strokeFR, strokeMini, elves, stroke500, strokeCombo, strokeM, other };

    [Flags]
    internal enum CommandFlags 
    {
        None = 0,
        CheckActive = 1, 
        Fiscal = 2, 
        TapePrinter = 4,
        CheckPaperStatus = 8
    };

    internal class DeviceErrorException : Exception
    {
        private short _errorCode;

        public short ErrorCode
        {
            get { return _errorCode; }
        }

        public DeviceErrorException(short errorCode)
        {
            _errorCode = errorCode;
        }
    }

    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypeStroke)]
    public class StrokeFiscalDevice : CustomFiscalDevice
    {
        #region Константы

        // наименования режимов ФР
        private string[] stateModes = new string[] {"Рабочий режим", "Выдача данных", 
            "Открытая смена, 24 часа не кончились", "Открытая смена, 24 часа кончились", "Закрытая смена",
            "Блокировка по неправильному паролю налогового инспектора", "Ожидание подтверждения ввода даты",
            "Разрешение изменения положения десятичной точки", "Открытый документ",
            "Режим разрешения технологического обнуления", "Тестовый прогон", 
            "Печать полного фискального отчета", "Печать отчёта ЭКЛЗ", 
            "Работа с фискальным подкладным документом", "Печать подкладного документа",
            "Фискальный подкладной документ сформирован"};

        // наименования подрежимов ФР
        private string[] stateSubModes = new string[] {"Продажа", "Покупка", "Возврат продажи", 
            "Возврат покупки", "Внесение денег", "Выплата денег", "Печать нефискального документа" };

        // пароль оператора по умолчанию
        private const uint DEF_OPERATOR_PASSWD = 30;

        // таймаут ожидания смены состояния подкладного документа
        private const long SLIP_WAIT_TIMEOUT = 3 * 60 * 1000;

        private const short PAPER_OUT_PASSIVE = 200;
        private const short PAPER_OUT_ACTIVE = 201;

        #endregion

        #region Поля

        private uint[] _paymentAmount = new uint[5]; 
        private StrokeType _deviceType;
        private DocumentType _currDocType = DocumentType.Other;
        private StrokeProtocol _protocol;
        private PrintableDeviceInfo _printerInfo = new PrintableDeviceInfo(new PrintableTapeWidth(40, 0),
            true);

        private int _maxSlipLines = 40;
        private int _slipLineNo = 0;
        private bool _firstSlip = false;
        private bool _needPrintTopMargin = false;

        #endregion

        #region Конструктор

        public StrokeFiscalDevice()
            : base()
        {
            AddSpecificError(1, "Неисправен накопитель ФП1, ФП2 или часы");
            AddSpecificError(2, "Отсутствует ФП1");
            AddSpecificError(3, "Отсутствует ФП2");
            AddSpecificError(4, "Некорректные параметры в команде обращения к ФП");
            AddSpecificError(5, "Нет запрошенных данных");
            AddSpecificError(6, "ФП в режиме вывода данных");
            AddSpecificError(7, "Некорректные параметры в команде для данной реализации ФП");
            AddSpecificError(8, "Команда не поддерживается в данной реализации ФП");
            AddSpecificError(9, "Некорректная длина команды");
            AddSpecificError(10, "Формат данных не BCD");
            AddSpecificError(11, "Неисправна ячейка памяти ФП при записи итога");
            AddSpecificError(17, "Не введена лицензия");
            AddSpecificError(18, "Заводской номер уже введен");
            AddSpecificError(19, "Текущая дата меньше даты последней записи в ФП");
            AddSpecificError(20, "Область сменных итогов ФП переполнена");
            AddSpecificError(21, "Смена уже открыта");
            AddSpecificError(22, "Смена не открыта");
            AddSpecificError(23, "Номер первой смены больше номера последней смены");
            AddSpecificError(24, "Дата первой смены больше даты последней смены");
            AddSpecificError(25, "Нет данных в ФП");
            AddSpecificError(26, "Область перерегистраций ФП переполнена");
            AddSpecificError(27, "Заводской номер не введен");
            AddSpecificError(28, "В заданном диапазоне есть поврежденная запись");
            AddSpecificError(29, "Повреждена последяя запись сменных итогов");
            AddSpecificError(30, "Область перерегистраций ФП переполнена");
            AddSpecificError(31, "Отсутствует память регистров");
            AddSpecificError(32, "Переполнение денежного регистра при добавлении");
            AddSpecificError(33, "Вычитаемая сумма больше содержимого денежного регистра");
            AddSpecificError(34, "Неверная дата");

            AddSpecificError(35, "Нет записи активизации");
            AddSpecificError(36, "Область активизаций переполнена");
            AddSpecificError(37, "Нет активизации с запращиваемым номером");
            AddSpecificError(38, "Вносимая клиентом сумма меньше суммы чека");
            AddSpecificError(43, "Невозможно отменить предыдущую команду");
            AddSpecificError(44, "Обнуленная касса (повторное гашение невозможно)");
            AddSpecificError(45, "Сумма чека по секции меньше суммы сторно");
            AddSpecificError(46, "В ФР нет денег для выплаты");
            AddSpecificError(48, "ФР заблокирован, ждет ввода пароля НИ");
            AddSpecificError(50, "Требуется выполнение общего гашения");

            AddSpecificError(51, "Некорректные параметры в команде");
            AddSpecificError(52, "Нет данных");
            AddSpecificError(53, "Некорректный параметр при данных настройках");
            AddSpecificError(54, "Некорректные параметры в команде для данной реализации ФР");
            AddSpecificError(55, "Команда не поддерживается в данной реализации ФР");
            AddSpecificError(56, "Ошибка в ПЗУ");
            AddSpecificError(57, "Внутренняя ошибка ПО ФР");
            AddSpecificError(58, "Переполнение накопления по надбавкам в смене");
            AddSpecificError(59, "Переполнение накопления в смене");
            AddSpecificError(60, "ЭКЛЗ: неверный регистрационный номер");
            AddSpecificError(61, "Смена не открыта - операция невозможна");

            AddSpecificError(62, "Переполнение накопления по секциям в смене");
            AddSpecificError(63, "Переполнение накопления по скидкам в смене");
            AddSpecificError(64, "Переполнение диапазона скидок");
            AddSpecificError(65, "Переполнение диапазона оплаты наличными");
            AddSpecificError(66, "Переполнение диапазона оплаты типом 2");
            AddSpecificError(67, "Переполнение диапазона оплаты типом 3");
            AddSpecificError(68, "Переполнение диапазона оплаты типом 4");
            AddSpecificError(69, "Сумма всех типов оплаты меньше итога чека");
            AddSpecificError(70, "Не хватает наличности в кассе");
            AddSpecificError(71, "Переполнение накопления по налогам в смене");
            AddSpecificError(72, "Переполнение итога чека");
            AddSpecificError(73, "Операция невозможна в открытом чеке данного типа");

            AddSpecificError(74, "Открыт чек - операция невозможна");
            AddSpecificError(75, "Буфер чека переполнен");
            AddSpecificError(76, "Переполнение накопления по обороту налогов в смене");
            AddSpecificError(77, "Вносимая безналичной оплатой сумма больше суммы чека");
            AddSpecificError(78, "Смена превысила 24 часа");
            AddSpecificError(79, "Неверный пароль");
            AddSpecificError(80, "Идет печать предыдущей команды");
            AddSpecificError(81, "Переполнение накоплений наличными в смене");
            AddSpecificError(82, "Переполнение накоплений по типу оплаты 2 в смене");
            AddSpecificError(83, "Переполнение накоплений по типу оплаты 3 в смене");
            AddSpecificError(84, "Переполнение накоплений по типу оплаты 4 в смене");
            AddSpecificError(85, "Чек закрыт - операция невозможна");

            AddSpecificError(86, "Нет документа для повтора");
            AddSpecificError(87, "ЭКЛЗ: количество закрытых смен не совпадает с ФП");

            AddSpecificError(88, "Ожидание команды продолжения печати");
            AddSpecificError(89, "Документ открыт другим оператором");
            AddSpecificError(90, "Скидка превышает накопления в чеке");

            AddSpecificError(91, "Переполнение диапазона надбавок");
            AddSpecificError(92, "Понижено напряжение 24В");
            AddSpecificError(93, "Таблица не определена");
            AddSpecificError(94, "Некорректная операция");
            AddSpecificError(95, "Отрицательный итог чека");
            AddSpecificError(96, "Переполнение при умножении");
            AddSpecificError(97, "Переполнение диапазона цены");
            AddSpecificError(98, "Переполнение диапазона количества");
            AddSpecificError(99, "Переполнение диапазона отдела");
            AddSpecificError(100, "ФП отсутствует");
            AddSpecificError(101, "Не хватает денег в секции");
            AddSpecificError(102, "Переполнение денег в секции");
            AddSpecificError(103, "Ошибка связи с ФП");
            AddSpecificError(104, "Не хватает денег по обороту налогов");
            AddSpecificError(105, "Переполнение по обороту налогов");
            AddSpecificError(106, "Ошибка питания в момент ответа по I2C");

            AddSpecificError(107, "Нет чековой ленты");
            AddSpecificError(108, "Нет контрольной ленты");
            AddSpecificError(109, "Не хватает денег по налогу");
            AddSpecificError(110, "Переполнение денег по налогу");
            AddSpecificError(111, "Переполнение по выплате в смене");
            AddSpecificError(112, "Переполнение ФП");
            AddSpecificError(113, "Ошибка отрезчика");
            AddSpecificError(114, "Команда не поддерживается в данном подрежиме");
            AddSpecificError(115, "Команда не поддерживается в данном режиме");
            AddSpecificError(116, "Ошибка ОЗУ");
            AddSpecificError(117, "Ошибка питания");
            AddSpecificError(118, "Ошибка принтера: нет импульсов с тахогенератора");
            AddSpecificError(119, "Ошибка принтера: нет сигнала с датчиков");
            AddSpecificError(120, "Замена ПО");
            AddSpecificError(121, "Замена ФП");
            AddSpecificError(122, "Поле не редактируется");
            AddSpecificError(123, "Ошибка оборудования");
            AddSpecificError(124, "Не совпадает дата");
            AddSpecificError(125, "Неверный формат даты");
            AddSpecificError(126, "Неверное значение в поле длины");
            AddSpecificError(127, "Переполнение диапазона итога чека");
            AddSpecificError(128, "Ошибка связи с ФП");
            AddSpecificError(129, "Ошибка связи с ФП");
            AddSpecificError(130, "Ошибка связи с ФП");
            AddSpecificError(131, "Ошибка связи с ФП");
            AddSpecificError(132, "Переполнение наличности");
            AddSpecificError(133, "Переполнение по продажам в смене");
            AddSpecificError(134, "Переполнение по покупкам в смене");
            AddSpecificError(135, "Переполнение по возвратам продаж в смене");
            AddSpecificError(136, "Переполнение по возвратам покупок в смене");
            AddSpecificError(137, "Переполнение по внесению в смене");
            AddSpecificError(138, "Переполнение по надбавкам в чеке");
            AddSpecificError(139, "Переполнение по скидкам в чеке");
            AddSpecificError(140, "Отрицательный итог надбавки в чеке");
            AddSpecificError(141, "Отрицательный итог скидки в чеке");
            AddSpecificError(142, "Нулевой итог чека");
            AddSpecificError(143, "Касса не фискализирована");
            AddSpecificError(144, "Поле превышает размер, установленный в настройках");
            AddSpecificError(145, "Выход за границу поля печати при данных настройках шрифта");
            AddSpecificError(146, "Наложение полей");
            AddSpecificError(147, "Восстановление ОЗУ прошло успешно");
            AddSpecificError(148, "Исчерпан лимит операций в чеке");
            AddSpecificError(160, "Ошибка связи с ЭКЛЗ");
            AddSpecificError(161, "ЭКЛЗ отсутствует");
            AddSpecificError(162, "ЭКЛЗ: некорректный формат или параметр команды");
            AddSpecificError(163, "Некорректное состояние ЭКЛЗ");
            AddSpecificError(164, "Авария ЭКЛЗ");
            AddSpecificError(165, "Авария КС в составе ЭКЛЗ");
            AddSpecificError(166, "Исчерпан временной ресурс ЭКЛЗ");
            AddSpecificError(167, "ЭКЛЗ переполнена");
            AddSpecificError(168, "ЭКЛЗ: неверные дата и время");
            AddSpecificError(169, "ЭКЛЗ: нет запрошенных данных");
            AddSpecificError(170, "Переполнение ЭКЛЗ (отрицательный итог документа)");
            AddSpecificError(176, "ЭКЛЗ: переполнение в параметре количество");
            AddSpecificError(177, "ЭКЛЗ: переполнение в параметре сумма");
            AddSpecificError(178, "ЭКЛЗ уже активизирована");
            AddSpecificError(192, "Контроль даты и времени (подтвердите дату и время)");
            AddSpecificError(193, "ЭКЛЗ: суточный отчет с гашением прервать нельзя");
            AddSpecificError(194, "Превышение напряжения в блоке питания");
            AddSpecificError(195, "Несовпадение итогов чека и ЭКЛЗ");
            AddSpecificError(196, "Несовпадение номеров смен");
            AddSpecificError(197, "Буфер подкладного документа пуст");
            AddSpecificError(198, "Подкладной документ отсутствует");
            AddSpecificError(199, "Поле не редактируется в данном режиме");

            AddSpecificError(PAPER_OUT_PASSIVE, "Отсутствует бумага в принтере");
            AddSpecificError(PAPER_OUT_ACTIVE, "Отсутствует бумага в принтере. Выполнение команды прервано");

            AddSpecificError(-1, "Неизвестная ошибка");
        }

        #endregion

        #region Внутренние методы

        // запись значения в таблицу ФР
        private void SetTableValue(byte table, int row, byte field, string value, int size)
        {            
            _protocol.CreateCommand(0x1E, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(table);
            _protocol.AddInt(row, 2);
            _protocol.AddByte(field);
            _protocol.AddString(value, size);
            _protocol.ExecuteCommand();
        }

        // Закрывает нефискальный документ
        private void CloseNonFiscalDoc()
        {
            // печать графического подвала
            DrawGraphicFooter();
            // печать подвала
            DrawFooter();
            // протяжка ленты
            _protocol.CreateCommand(0x29, DEF_OPERATOR_PASSWD);
            _protocol.AddBytes(0x02, 0x04);
            _protocol.ExecuteCommand();
            // отрезка ленты
            _protocol.CreateCommand(0x25, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(1);
            _protocol.ExecuteCommand();
            // печать строк клише
            DrawHeader();
        }

        // Закрытие фискального документа с проверкой сквозного номера чека
        private void CloseFiscalDoc()
        {
            // сохраняем текущий сквозной номер документа
            int currReceiptNo = _protocol.GetReceiptNo(DEF_OPERATOR_PASSWD);

            try
            {
                _protocol.CreateCommand(0x85, DEF_OPERATOR_PASSWD);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Cash], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Card], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Other1], 5);
                _protocol.AddInt(_paymentAmount[(int)FiscalPaymentType.Other2], 5);
                _protocol.AddInt(0, 2);
                _protocol.AddEmptyBytes(4);
                _protocol.AddString(string.Empty, 40);
                _protocol.ExecuteCommand(true);
            }
            catch (TimeoutException)
            {
                _protocol.WriteDebugLine("Таймаут передачи данных при выполнении команды закрытия документа");
                _protocol.WriteDebugLine("Сохраненный сквозной номер документа: " + currReceiptNo);
                // проверка номера документа
                int newReceiptNo = _protocol.GetReceiptNo(DEF_OPERATOR_PASSWD);
                _protocol.WriteDebugLine("Текущий сквозной номер документа: " + newReceiptNo);
                if (newReceiptNo > currReceiptNo)
                    _protocol.WriteDebugLine("Документ был закрыт на ФР");
                else
                {
                    _protocol.WriteDebugLine("Документ не был закрыт на ФР");
                    throw;
                }
            }
        }

        // Вызывается после завершения печати любого документа
        private void DoAfterClose()
        {
            // печать графического заголовка
            DrawGraphicHeader();
            // обнуление счетчиков
            _paymentAmount = new uint[5]; 
            _currDocType = DocumentType.Other;
        }

        // Отмена документа, если он открыт
        private void CancelOpenedDocument(bool fiscal)
        {
            StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
            if (printerFlags.Mode == 8 && fiscal)
                // команда отмены документа
                _protocol.ExecuteCommand(0x88, DEF_OPERATOR_PASSWD, true);
            else if (printerFlags.Mode != 8 && !fiscal)
            {
                _protocol.CreateCommand(0x17, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(2);
                _protocol.AddString("ЧЕК АННУЛИРОВАН", 40);
                _protocol.ExecuteCommand();
                CloseNonFiscalDoc();
            }
            else
                return;
            DoAfterClose();
        }

        // Определение типа устройства и установка длины строки
        private void SetDeviceType()
        {
            _protocol.ExecuteCommand(0xFC);
            if (_protocol.Response[2] == 0)
            {
                switch (_protocol.Response[6])
                {
                    case 0:
                    case 1:
                    case 4: // Штрих-ФР и его модификации
                        _deviceType = StrokeType.strokeFR;
                        PrinterInfo.TapeWidth.MainPrinter = 36;
                        break;
                    case 2:
                    case 6: // Элвес-Мини
                        _deviceType = StrokeType.elves;
                        PrinterInfo.TapeWidth.MainPrinter = 32;
                        break;
                    case 7: // Штрих-Мини
                        _deviceType = StrokeType.strokeMini;
                        PrinterInfo.TapeWidth.MainPrinter = 50;
                        break;
                    case 9: // Штрих-Комбо-ФР-К
                    case 12: // Штрих-Комбо-ФР-К (версия 02)
                        _deviceType = StrokeType.strokeCombo;
                        PrinterInfo.TapeWidth.MainPrinter = 48;
                        PrinterInfo.TapeWidth.AdditionalPrinter1 = 48;
                        int lineInterval = 24;
                        double Y_UNIT_SIZE = 0.176;
                        if (_printerInfo.SlipFormLength > 0)
                            _maxSlipLines = (int)((_printerInfo.SlipFormLength - 20) / (Y_UNIT_SIZE * lineInterval));
                        break;
                    case 250: // Штрих-М
                        _deviceType = StrokeType.strokeM;
                        PrinterInfo.TapeWidth.MainPrinter = 48;
                        break;
                    default: // прочие модели регистраторов
                        _deviceType = StrokeType.other;
                        PrinterInfo.TapeWidth.MainPrinter = 50;
                        break;
                }
            }
            else if (_protocol.Response[2] == 5)
            {
                PrinterInfo.TapeWidth.MainPrinter = 32;
                _deviceType = StrokeType.stroke500;
            }
        }

        // Проверка состояния бумаги и установка кода ошибки
        private void CheckPaperStatus()
        {
            switch (_protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).tapePaperStatus)
            {
                case PaperOutStatus.OutPassive:
                    // бумага отсутствует
                    throw new DeviceErrorException(PAPER_OUT_PASSIVE);
                case PaperOutStatus.OutActive:
                    // бумага отсутствует, команда не завершена
                    throw new DeviceErrorException(PAPER_OUT_ACTIVE);
                case PaperOutStatus.OutAfterActive:
                    // бумага присутствует, необходимо выполнить команду продолжения печати
                    _protocol.ExecuteCommand(0xB0, DEF_OPERATOR_PASSWD, true);
                    break;
            }
        }

        // получение байтов для печати изображения
        private byte[] GetImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            byte[] imageBytes = new byte[image.Height * bmpData.Stride];
            stride = bmpData.Stride;
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Height * stride);
                if ((ulong)image.Palette.Entries[0].ToArgb() == 0xff000000)
                {
                    for (int rowNo = 0; rowNo < image.Height; rowNo++)
                        for (int i = 0; i < stride; i++)
                        {
                            if (i * 8 < image.Width)
                                imageBytes[rowNo * stride + i] = (byte)(imageBytes[rowNo * stride + i] ^ 0xFF);
                            else
                            {
                                // инвентируем биты выходящие за границу изображения
                                int bitsInvert = (i * 8) - image.Width;
                                int invertMask = 0xFF >> (8 - bitsInvert);
                                imageBytes[rowNo * stride + i] = (byte)(imageBytes[rowNo * stride + i] ^ invertMask);
                            }
                        }
                }
                return imageBytes;
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        // получение байтов для печати изображения
        private byte[] GetColorImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(image.Width, image.Height, PixelFormat.Format1bppIndexed);
            BitmapData bmpData = newImage.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), 
                ImageLockMode.ReadOnly, newImage.PixelFormat);
            stride = bmpData.Stride;
            newImage.UnlockBits(bmpData);

            byte[] imageBytes = new byte[newImage.Height * stride];
            for (int y = 0; y < image.Height; y++)
            {
                for (int i = 0; i < stride; i++)
                {
                    byte b = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        if (i * 8 + x >= image.Width)
                            break;
                        if ((ulong)image.GetPixel(i * 8 + x, y).ToArgb() != 0xffffffff)
                            b += (byte)(0x01 << (7 - x));
                    }
                    imageBytes[y * stride + i] = b;
                }
            }
            return imageBytes;
        }

        private bool WaitForSlipState(bool needState, long slipWaitTimeout)
        {
            long waitTimeout = slipWaitTimeout * 10000;
            long startTicks = DateTime.Now.Ticks;
            StrokePrinterFlags printerFlags = new StrokePrinterFlags();
            do
            {
                printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                if (printerFlags.slipPaperPresent != needState)
                    System.Threading.Thread.Sleep(100);
                else
                    return true;
            }
            while (DateTime.Now.Ticks - startTicks < waitTimeout);
            //m_nErrorCode = PAPER_OUT_PASSIVE;
            return false;
        }

        private void StartSlip()
        {
            // очистить буфер подкладного документа
            _protocol.ExecuteCommand(0x7C, DEF_OPERATOR_PASSWD);
            _firstSlip = true;
            _slipLineNo = 1;
        }

        private void EndSlip()
        {
            // если печатать нечего, выходим
            if (_slipLineNo <= 1)
                return;
            // ожидаем пока уберут прежний документ
            if (!_firstSlip && !WaitForSlipState(false, SLIP_WAIT_TIMEOUT))
                return;
            // ожидаем пока вставят новый документ
            if (!WaitForSlipState(true, _firstSlip ? 0 : SLIP_WAIT_TIMEOUT))
                return;
            // печатать буфер подкладного документа
            PrintSlipBuffer();

            // ожидаем завершения печати
            while (_protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).Mode == 14)
                System.Threading.Thread.Sleep(100);

            _needPrintTopMargin = true;
        }

        private void PrintSlipBuffer()
        {
            _firstSlip = false;
            _slipLineNo = 1;
            _protocol.CreateCommand(0x7D, DEF_OPERATOR_PASSWD);
            _protocol.AddBytes(0x00, 0x02);
            _protocol.ExecuteCommand();

            // очистка всего буфера
//            _protocol.ExecuteCommand(0x7C, DEF_OPERATOR_PASSWD);
        }

        private void PrintSlipLine(string source, FontStyle style)
        {
            // печать верхнего оступа
            if (_needPrintTopMargin)
            {
                _needPrintTopMargin = false;
                PrintTopMargin();
            }

            // если _slipLineNo == 0, вызываем

            // добавляем строку подкладного документа
            _protocol.CreateCommand(0x7A, DEF_OPERATOR_PASSWD);
            _protocol.AddByte(_slipLineNo++);
            _protocol.AddByte(0x1B);
            // установка шрифта
            _protocol.AddByte((style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll) ? 0x02 : 0x01);
            _protocol.AddString(source, (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) 
                ? PrinterInfo.TapeWidth.AdditionalPrinter1 / 2 : PrinterInfo.TapeWidth.AdditionalPrinter1);
            _protocol.ExecuteCommand();

            // если буффер документа заполнен, то печатаем его
            if (_slipLineNo > _maxSlipLines)
                EndSlip();
        }

        #endregion

        private delegate void ExecuteCommandDelegate();

        private void ExecuteDriverCommand(CommandFlags commandFlags, ExecuteCommandDelegate executeCommandDelegate)
        {
            try
            {
                this.ErrorCode = new ServerErrorCode(this, GeneralError.Success);
                // проверка активности устройства
                if ((commandFlags & CommandFlags.CheckActive) == CommandFlags.CheckActive)
                    if (!Active)
                    {
                        this.ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                        return;
                    }

                // команда не поддерживается принтером "Штрих-500"
                if ((commandFlags & CommandFlags.Fiscal) == CommandFlags.Fiscal)
                    if (_deviceType == StrokeType.stroke500)
                        return;

                // команда не поддерживается подкладным принтером
                if ((commandFlags & CommandFlags.TapePrinter) == CommandFlags.TapePrinter)
                    if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                        return;

                executeCommandDelegate();

                //if ((commandFlags & CommandFlags.CheckPaperStatus) == CommandFlags.CheckPaperStatus)
                //    CheckPaperStatus();
            }
            catch (DeviceErrorException E)
            {
                if (E.ErrorCode >= 0x1000)
                {
                    this.ErrorCode = new ServerErrorCode(this, (GeneralError)E.ErrorCode);
                    return;
                }
                string message = GetSpecificDescription(E.ErrorCode);
                string dumpStr = _protocol.GetDumpStr();
                // для ошибки "Команда недопустима в данном режиме" дополнительно возвращаем режим
                if (E.ErrorCode == 115)
                {
                    StrokePrinterFlags printerFlags;
                    if (_protocol.TryGetPrinterFlags(DEF_OPERATOR_PASSWD, out printerFlags))    
                        message = String.Format("{0}. Режим: {1}. Подрежим: {2}", message, stateModes[printerFlags.Mode], stateSubModes[printerFlags.Submode]);
                }
                this.ErrorCode = new ServerErrorCode(this, E.ErrorCode, message, dumpStr);

                // отмена открытого нефискального документа
                try
                {
                    CancelOpenedDocument(false);
                }
                catch (Exception)
                {
                }
            }
            catch (TimeoutException)
            {
                this.ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _protocol.GetDumpStr());
            }
            catch (Exception E)
            {
                this.ErrorCode = new ServerErrorCode(this, E);
            }
            finally
            {
                if (!ErrorCode.Succeeded && Logger.DebugInfo && !String.IsNullOrEmpty(_protocol.DebugInfo))
                    Logger.SaveDebugInfo(this, _protocol.DebugInfo);

                _protocol.ClearDebugInfo();
            }
        }

        #region Реализация виртуальных функций

        protected override void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
            DCB dcb = e.DCB;
            dcb.XonChar = 0x11;
            dcb.XoffChar = 0x13;
            dcb.XonLim = 2048;
            dcb.XoffLim = 512;

            dcb.fDtrControl = (uint)DtrControl.Disable;
            dcb.fAbortOnError = 0;
            dcb.fDsrSensitivity = 0;
            dcb.fOutxDsrFlow = 0;
            dcb.fOutxCtsFlow = 0;

            dcb.fTXContinueOnXoff = 0;
            dcb.fInX = 0;
            dcb.fOutX = 0;
            dcb.fRtsControl = (uint)RtsControl.Disable;
            dcb.fNull = 0;

            e.DCB = dcb;
            e.Handled = true;
            base.SetCommStateEventHandler(sender, e);
        }

        protected override void OnAfterActivate()
        {
            _protocol = new StrokeProtocol(Port);            
            ExecuteDriverCommand(CommandFlags.None, delegate()
            {
                // определение типа устройства
                SetDeviceType();
                // проверка состояния бумаги
                CheckPaperStatus();
                // отмена открытого документа
                CancelOpenedDocument(true);
                // установка клише и подвала
                if (!(_deviceType == StrokeType.stroke500 || _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD).OpenedShift))
                {
                    // установка заголовка
                    if (DocumentHeader != null)
                    {
                        int firstRow = (_deviceType == StrokeType.strokeM) ? 11 : 6;

                        for (int i = 0; i < 4; i++)
                            SetTableValue(4, firstRow + i, 1, i < DocumentHeader.Length ? DocumentHeader[i] : string.Empty, 
                                PrinterInfo.TapeWidth.MainPrinter);
                    }

                    // установка подвала
                    if (DocumentFooter != null)
                        for (int i = 0; i < 3; i++)
                            SetTableValue(4, i + 1, 1, i < DocumentFooter.Length ? DocumentFooter[i] : string.Empty,
                                PrinterInfo.TapeWidth.MainPrinter);
                }
            });
        }

        protected override void OnOpenDocument(DocumentType docType,
            String cashierName)
        {
            FiscalStatusFlags fsFlags = Status;
            _needPrintTopMargin = false;
            _protocol.ClearDebugInfo();
            _protocol.WriteDebugLine("OnOpenDocument");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                    StartSlip();

                // проверка состояния бумаги
                CheckPaperStatus();
                
                // отмена открытого документа
                CancelOpenedDocument(true);
                
                // установка имени кассира
                SetTableValue(2, 30, 2, cashierName, 21);
                
                // сохраняем тип документа
                _currDocType = docType;

                if (_currDocType == DocumentType.Sale || _currDocType == DocumentType.Refund)
                    if (this.HasNonzeroRegistrations)
                    {
                        _protocol.CreateCommand(0x8D, DEF_OPERATOR_PASSWD);
                        _protocol.AddByte(docType == DocumentType.Sale ? 0 : 2);
                        _protocol.ExecuteCommand();
                    }

            });
        }

        protected override void OnCustomTopMarginPrinted()
        {
            if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                // добавить клише
                DrawHeader();
        }

        protected override void OnCloseDocument(bool cutPaper)
        {
            _protocol.WriteDebugLine("OnCloseDocument");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                {
                    // добавить подвал
                    DrawFooter();
                    EndSlip();
                    return;
                }

                switch (_currDocType)
                {
                    case DocumentType.Sale: // продажа
                    case DocumentType.Refund: // возврат
                        StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
                        if (printerFlags.Mode != 8) // если фискальный документ не был открыт
                            CloseNonFiscalDoc();
                        else
                            CloseFiscalDoc();
                        break;
                    case DocumentType.PayingIn: // внесение
                    case DocumentType.PayingOut: // выплата
                        _protocol.CreateCommand(_currDocType == DocumentType.PayingIn ? (byte)0x50 : (byte)0x51,
                            DEF_OPERATOR_PASSWD);
                        _protocol.AddInt(_paymentAmount[(uint)FiscalPaymentType.Cash], 5);
                        _protocol.ExecuteCommand(true);
                        break;
                    case DocumentType.XReport:  // X-отчет
                        _protocol.ExecuteCommand(0x40, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.ZReport:  // Z-отчет
                        _protocol.ExecuteCommand(0x41, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.SectionsReport:   // отчет по секциям
                        _protocol.ExecuteCommand(0x42, DEF_OPERATOR_PASSWD, true);
                        break;
                    case DocumentType.Other: // нефискальный документ
                        CloseNonFiscalDoc();
                        break;
                }
                DoAfterClose();
            });
        }

        protected override void OnOpenDrawer()
        {
            _protocol.WriteDebugLine("OnOpenDrawer");
            ExecuteDriverCommand(CommandFlags.CheckActive, delegate()
            {
                _protocol.CreateCommand(0x28, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(0);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPrintString(String source, FontStyle style)
        {
            _protocol.WriteDebugLine("OnPrintString");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.CheckPaperStatus, delegate()
            {
                if (PrinterNumber == PrinterNumber.AdditionalPrinter1 && _deviceType == StrokeType.strokeCombo)
                    PrintSlipLine(source, style);
                else
                {
                    _protocol.CreateCommand(style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll ?
                        (byte)0x12 : (byte)0x17, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(2);
                    int strLen = (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) ? PrinterInfo.TapeWidth.MainPrinter / 2 : PrinterInfo.TapeWidth.MainPrinter;
                    int maxStrLen = (style == FontStyle.DoubleWidth) || (style == FontStyle.DoubleAll) ? 20 : 40;
                    _protocol.AddString(source, strLen < maxStrLen ? maxStrLen : strLen);
                    _protocol.ExecuteCommand(true);
                }
            });
        }

        protected override void OnPrintBarcode(String barcode, AlignOptions align,
            bool readable)
        {
            _protocol.WriteDebugLine("OnPrintBarcode");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                _protocol.CreateCommand(0xC2, DEF_OPERATOR_PASSWD);
                _protocol.AddInt(Convert.ToInt64(barcode), 5);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            _protocol.WriteDebugLine("OnPrintImage");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                int stride = 0;
                byte[] imageBytes = image.PixelFormat == PixelFormat.Format1bppIndexed ?
                    GetImageBytes(image, out stride) :
                    GetColorImageBytes(image, out stride);

                int leftBytesCount = 0;
                int rightBytesCount = 0;
                if (stride < 40)
                    switch (align)
                    {
                        case AlignOptions.Left:
                            rightBytesCount = 40 - stride;
                            break;
                        case AlignOptions.Center:
                            leftBytesCount = (40 - stride) / 2;
                            rightBytesCount = 40 - stride - leftBytesCount;
                            break;
                        case AlignOptions.Right:
                            leftBytesCount = 40 - stride;
                            break;
                    }

                for (int row = 0; (row < image.Height) && (row < 200); row++)
                {
                    _protocol.CreateCommand(0xC0, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(row);
                    _protocol.AddEmptyBytes(leftBytesCount);
                    for (int i = 0; (i < stride) && (i < 40); i++)
                    {
                        byte b = imageBytes[row * stride + i];
                        // переворачивание байта
                        ulong n = (((b * 0x0802LU & 0x22110LU) | (b * 0x8020LU & 0x88440LU)) * 0x10101LU >> 16);
                        _protocol.AddByte((int)n);
                    }
                    _protocol.AddEmptyBytes(rightBytesCount);
                    _protocol.ExecuteCommand();
                }

                // печать графики
                _protocol.CreateCommand(0xC1, DEF_OPERATOR_PASSWD);
                _protocol.AddByte(1);
                _protocol.AddByte(Math.Max(image.Height, 199) + 1);
                _protocol.ExecuteCommand();
            });
        }

        protected override void OnRegistration(String commentary, UInt32 quantity, UInt32 amount,
            Byte section)
        {
            _protocol.WriteDebugLine("OnRegistration");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter | CommandFlags.CheckPaperStatus, delegate()
            {
                StrokePrinterFlags printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                // фискальный документ, продажа
                if (printerFlags.Mode == 8 && printerFlags.StateMode == 0)
                    _protocol.CreateCommand(0x80, DEF_OPERATOR_PASSWD);
                // фискальный документ, возврат
                else if (printerFlags.Mode == 8 && printerFlags.StateMode == 2)
                    _protocol.CreateCommand(0x82, DEF_OPERATOR_PASSWD);
                // нефискальный документ
                else
                    return;

                _protocol.AddInt(quantity, 5);
                _protocol.AddInt(amount, 5);
                _protocol.AddByte(section);
                _protocol.AddEmptyBytes(4);
                _protocol.AddString(commentary, 40);

                _protocol.ExecuteCommand();
            });
        }

        protected override void OnPayment(UInt32 amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter, delegate()
            {
                _paymentAmount[(int)paymentType] += amount;
            });
        }

        protected override void OnCash(UInt32 amount)
        {
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.TapePrinter, delegate()
            {
                _paymentAmount[(int)FiscalPaymentType.Cash] += amount;
            });
        }

        protected override void OnContinuePrint()
        {
            _protocol.WriteDebugLine("OnContinuePrint");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.TapePrinter, delegate()
            {
                _protocol.ExecuteCommand(0xB0, DEF_OPERATOR_PASSWD, true);
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
                _protocol.WriteDebugLine("CurrentTimestamp_get");
                DateTime currentTimestamp = DateTime.Now;
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    currentTimestamp = new DateTime(
                        _protocol.Response[27] + 2000, _protocol.Response[26], _protocol.Response[25],
                        _protocol.Response[28], _protocol.Response[29], _protocol.Response[30]);
                });
                return currentTimestamp;
            }
            set
            {
                _protocol.WriteDebugLine("CurrentTimestamp_set");
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    // установка времени
                    _protocol.CreateCommand(0x21, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Hour, value.Minute, value.Second);
                    _protocol.ExecuteCommand();
                    // установка даты
                    _protocol.CreateCommand(0x22, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Day, value.Month, value.Year % 100);
                    _protocol.ExecuteCommand();
                    // подтверждение изменения даты
                    _protocol.CreateCommand(0x23, DEF_OPERATOR_PASSWD);
                    _protocol.AddBytes(value.Day, value.Month, value.Year % 100);
                    _protocol.ExecuteCommand();
                });
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get { return _printerInfo; }
        }

        public override FiscalDeviceInfo Info
        {
            get
            {
                _protocol.WriteDebugLine("Info_get");
                string SerialNo = "";
                string DeviceType = DeviceNames.ecrTypeStroke;

                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    SerialNo = _protocol.GetInt(32, 4).ToString();
                });
                return new FiscalDeviceInfo(DeviceType, SerialNo);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                _protocol.WriteDebugLine("PrinterStatus_get");
                StrokePrinterFlags printerFlags = new StrokePrinterFlags();
                ExecuteDriverCommand(CommandFlags.CheckActive, delegate()
                {
                    printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);
                });
                return new PrinterStatusFlags(printerFlags.Printing, printerFlags.tapePaperStatus, 
                    printerFlags.Mode == 8, printerFlags.drawerOpened);
            }
        }

        public override FiscalStatusFlags Status
        {
            get
            {
                _protocol.WriteDebugLine("Status_get");
                bool bFiscalized = false;
                ulong nDocumentAmount = 0;
                ulong nCashInDrawer = 0;
                StrokePrinterFlags printerFlags = new StrokePrinterFlags();
                ExecuteDriverCommand(CommandFlags.Fiscal, delegate()
                {
                    printerFlags = _protocol.GetPrinterFlags(DEF_OPERATOR_PASSWD);

                    _protocol.ExecuteCommand(0x11, DEF_OPERATOR_PASSWD);
                    bFiscalized = _protocol.Response[40] > 0;

                    _protocol.CreateCommand(0x1A, DEF_OPERATOR_PASSWD);
                    _protocol.AddByte(241);
                    _protocol.ExecuteCommand();
                    nCashInDrawer = (ulong)_protocol.GetInt(3, 6);

                    // считаем сумму документа по всем регистрам
                    int nRegister = 0;
                    // если открыт фискальный документ
                    if (printerFlags.Mode == 8)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            _protocol.CreateCommand(0x1A, DEF_OPERATOR_PASSWD);
                            nRegister += 4;
                            _protocol.AddByte(nRegister);
                            _protocol.ExecuteCommand();
                            nDocumentAmount += (ulong)_protocol.GetInt(3, 6);
                        }
                    }
                });
                return new FiscalStatusFlags(printerFlags.OpenedShift, printerFlags.OverShift, 
                    printerFlags.Locked, bFiscalized, nDocumentAmount, nCashInDrawer);
            }
        }

        #endregion

        #region Методы

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            _protocol.WriteDebugLine("FiscalReport");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal | CommandFlags.CheckPaperStatus, delegate()
            {
                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        DateTime dtParam = (DateTime)(reportParams[0]);
                        _protocol.CreateCommand(0x66, TaxerPassword);
                        _protocol.AddByte(Convert.ToByte(full));
                        _protocol.AddBytes(dtParam.Day, dtParam.Month, dtParam.Year % 100);
                        dtParam = (DateTime)(reportParams[1]);
                        _protocol.AddBytes(dtParam.Day, dtParam.Month, dtParam.Year % 100);
                        break;
                    case FiscalReportType.ByShifts:
                        _protocol.CreateCommand(0x67, TaxerPassword);
                        _protocol.AddByte(Convert.ToByte(full));
                        _protocol.AddInt((int)(reportParams[0]), 2);
                        _protocol.AddInt((int)(reportParams[1]), 2);
                        break;
                }
                _protocol.ExecuteCommand(true);
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            _protocol.WriteDebugLine("Fiscalization");
            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal, delegate()
            {
                _protocol.CreateCommand(0x65, TaxerPassword);
                _protocol.AddInt(newPassword, 4);
                _protocol.AddInt(registrationNumber, 5);
                _protocol.AddInt(taxPayerNumber, 6);
                _protocol.ExecuteCommand(true);
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            _protocol.WriteDebugLine("GetLifetime");
            DateTime _firstDate = DateTime.Now;
            DateTime _lastDate = DateTime.Now;
            int _firstShift = 1;
            int _lastShift = 9999;

            ExecuteDriverCommand(CommandFlags.CheckActive | CommandFlags.Fiscal, delegate()
            {
                _protocol.ExecuteCommand(0x64, TaxerPassword);
                _firstDate = new DateTime(_protocol.Response[4] + 2000, _protocol.Response[3], _protocol.Response[2]);
                _lastDate = new DateTime(_protocol.Response[7] + 2000, _protocol.Response[6], _protocol.Response[5]);
                _firstShift = (int)_protocol.GetInt(8, 2);
                _lastShift = (int)_protocol.GetInt(10, 2);
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
