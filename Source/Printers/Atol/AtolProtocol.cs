using System;
using System.Drawing.Imaging;
using System.Linq;
using DevicesBase;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace Atol
{
    internal enum AtolModel
    {
        TriumfR     = 13,
        FelixRF     = 14, 
        Felix02K    = 15,
        Mercury140F = 16,
        Tornado     = 20,
        MercuryMSK  = 23,
        FelixRK     = 24,
        Felix3SK    = 27,
        FPrint02K   = 30,
        FPrint03K   = 31,
        FPrint88K   = 32,
        FPrint5200K = 35,
        FPrint55K   = 47,
        FPrint22K   = 52,
        FPrint22PTK = 63,
        Unknown     = 255
    }

    internal struct DeviceParams
    {
        // максимальная длина передаваемой строки
        public int MaxStringLen;

        // максимальное количество печатаемых символов
        public int StringLen;

        // ширина чековой ленты в точках
        public int TapeWidth;

        // множитель ширины штрих-кода
        public int BarcodeWidth;

        // кол-во символов подкладного документа
        public int SlipLineSize;

        // поддержка отрезчика
        public bool IsCutterSupported;
    }

    [Serializable]
    [FiscalDeviceAttribute(DeviceNames.ecrTypeAtol)]
    public class AtolFiscalDevice : CustomFiscalDevice, ICommunicationPortProvider
    {
        #region Константы

        private const int READ_TIMEOUT = 1000;
        private const int WRITE_TIMEOUT = 200;

        // пароль доступа к ККМ
        private const int OPERATOR_PASSWD = 1111;

        // пароль переключения режимов
        private const int MODE_PASSWD = 30;

        // следующие константы нужны для формирования штрих-кода
        private const int ODD = 0;
        private const int EVEN = 1;
        private const int RIGHT = 2;

        private string[][] EAN_CHARSET = new string[][] {
            new string[] {"0001101", "0100111", "1110010"},
            new string[] {"0011001", "0110011", "1100110"},
            new string[] {"0010011", "0011011", "1101100"},
            new string[] {"0111101", "0100001", "1000010"},
            new string[] {"0100011", "0011101", "1011100"},
            new string[] {"0110001", "0111001", "1001110"},
            new string[] {"0101111", "0000101", "1010000"},
            new string[] {"0111011", "0010001", "1000100"},
            new string[] {"0110111", "0001001", "1001000"},
            new string[] {"0001011", "0010111", "1110100"}};

        private int[][] EAN_PARITY = new int[][] {
            new int[] {ODD, ODD,  ODD,  ODD,  ODD,  ODD },
            new int[] {ODD, ODD,  EVEN, ODD,  EVEN, EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, ODD,  EVEN},
            new int[] {ODD, ODD,  EVEN, EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, ODD,  ODD,  EVEN, EVEN},
            new int[] {ODD, EVEN, EVEN, ODD,  ODD,  EVEN},
            new int[] {ODD, EVEN, EVEN, EVEN, ODD,  ODD },
            new int[] {ODD, EVEN, ODD,  EVEN, ODD,  EVEN},
            new int[] {ODD, EVEN, ODD,  EVEN, EVEN, ODD },
            new int[] {ODD, EVEN, EVEN, ODD,  EVEN, ODD }
        };

        #endregion

        #region Поля класса

        // тип текущего документа
        private DocumentType _currDocType;

        // сумма чека (при внесении/выплате денег и регистрации по 54-ФЗ)
        private long _docAmount = 0;

        // модель устройства
        private AtolModel _deviceModel = AtolModel.Unknown;

        private AtolProtocol _atolProtocol;

        #endregion

        #region Конструктор

        public AtolFiscalDevice()
            : base()
        {
            AddSpecificError(1, "Контрольная лента обработана без ошибок");
            AddSpecificError(8, "Неверная цена (сумма)");
            AddSpecificError(10, "Неверное количество");
            AddSpecificError(11, "Переполнение счетчика наличности");
            AddSpecificError(12, "Невозможно сторно последней операции");
            AddSpecificError(13, "Сторно по коду невозможно (в чеке зарегистрировано меньшее количество товаров с указанным кодом)");
            AddSpecificError(14, "Невозможен повтор последней операции");
            AddSpecificError(15, "Повторная скидка на операцию невозможна");
            AddSpecificError(16, "Скидка/надбавка на предыдущую операцию невозможна");
            AddSpecificError(17, "Неверный код товара");
            AddSpecificError(18, "Неверный штрих-код товара");
            AddSpecificError(19, "Неверный формат");
            AddSpecificError(20, "Неверная длина");
            AddSpecificError(21, "ККМ заблокирована в режиме ввода даты");
            AddSpecificError(22, "Требуется подтверждение ввода даты");
            AddSpecificError(24, "Нет больше данных для передачи ПО ККМ");
            AddSpecificError(25, "Нет подтверждения или отмены продажи");
            AddSpecificError(26, "Отчет с гашением прерван. Вход в режим невозможен");
            AddSpecificError(27, "Отключение контроля наличности невозможно (не настроены необходимые типы оплаты)");
            AddSpecificError(30, "Вход в режим заблокирован");
            AddSpecificError(31, "Проверьте дату и время");
            AddSpecificError(32, "Дата и время ККМ меньше, чем в ЭКЛЗ");
            AddSpecificError(33, "Невозможно закрыть архив");
            AddSpecificError(50, "Переполнение таблицы заказов");
            AddSpecificError(51, "Невозможен возврат товара");
            AddSpecificError(61, "Товар не найден");
            AddSpecificError(62, "Весовой штрих-код с количеством <>1.000");
            AddSpecificError(63, "Переполнение буфера чека");
            AddSpecificError(64, "Недостаточное количество товара");
            AddSpecificError(65, "Сторнируемое количество больше проданного");
            AddSpecificError(66, "Заблокированный товар не найден в буфере чека");
            AddSpecificError(67, "Данный товар не продавался в чеке, сторно невозможно");
            AddSpecificError(68, "Memo Plus 3 заблокировано с ПК");
            AddSpecificError(69, "Ошибка контрольной суммы таблицы настроек Memo Plus 3");
            AddSpecificError(70, "Неверная команда от ККМ");
            AddSpecificError(102, "Команда не реализуется в данном режиме ККМ");
            AddSpecificError(103, "Нет бумаги");
            AddSpecificError(104, "Нет связи с принтером чеков");
            AddSpecificError(105, "Механическая ошибка печатающего устройства");
            AddSpecificError(106, "Неверный тип чека");
            AddSpecificError(107, "Нет больше строк картинки");
            AddSpecificError(108, "Неверный номер регистра");
            AddSpecificError(109, "Недопустимое целевое устройство");
            AddSpecificError(110, "Нет места в массиве картинок");
            AddSpecificError(111, "Неверный номер картинки / картинка отсутствует");
            AddSpecificError(112, "Сумма сторно больше, чем было получено данным типом оплаты");
            AddSpecificError(113, "Сумма не наличных платежей превышает сумму чека");
            AddSpecificError(114, "Сумма платежей меньше суммы чека");
            AddSpecificError(115, "Накопление меньше суммы возврата или аннулирования");
            AddSpecificError(117, "Переполнение суммы платежей");
            AddSpecificError(122, "Данная модель ККМ не может выполнить команду");
            AddSpecificError(123, "Неверная величина скидки / надбавки");
            AddSpecificError(124, "Операция после скидки / надбавки невозможна");
            AddSpecificError(125, "Неверная секция");
            AddSpecificError(126, "Неверный вид оплаты");
            AddSpecificError(127, "Переполнение при умножении");
            AddSpecificError(128, "Операция запрещена в таблице настроек");
            AddSpecificError(129, "Переполнение итога чека");
            AddSpecificError(130, "Открыт чек аннулирования – операция невозможна");
            AddSpecificError(132, "Переполнение буфера контрольной ленты");
            AddSpecificError(134, "Вносимая клиентом сумма меньше суммы чека");
            AddSpecificError(135, "Открыт чек возврата – операция невозможна");
            AddSpecificError(136, "Смена превысила 24 часа");
            AddSpecificError(137, "Открыт чек продажи – операция невозможна");
            AddSpecificError(138, "Переполнение ФП");
            AddSpecificError(140, "Неверный пароль");
            AddSpecificError(141, "Буфер контрольной ленты не переполнен");
            AddSpecificError(142, "Идет обработка контрольной ленты");
            AddSpecificError(143, "Обнуленная касса (повторное гашение невозможно)");
            AddSpecificError(145, "Неверный номер таблицы");
            AddSpecificError(146, "Неверный номер ряда");
            AddSpecificError(147, "Неверный номер поля");
            AddSpecificError(148, "Неверная дата");
            AddSpecificError(149, "Неверное время");
            AddSpecificError(150, "Сумма чека по секции меньше суммы сторно");
            AddSpecificError(151, "Подсчет суммы сдачи невозможен");
            AddSpecificError(152, "В ККМ нет денег для выплаты");
            AddSpecificError(154, "Чек закрыт – операция невозможна");
            AddSpecificError(155, "Чек открыт – операция невозможна");
            AddSpecificError(156, "Смена открыта, операция невозможна");
            AddSpecificError(157, "ККМ заблокирована, ждет ввода пароля налогового инспектора");
            AddSpecificError(158, "Заводской номер уже задан");
            AddSpecificError(159, "Количество перерегистраций не может быть более 4");
            AddSpecificError(160, "Ошибка Ф.П.");
            AddSpecificError(162, "Неверная смена");
            AddSpecificError(163, "Неверный тип отчета");
            AddSpecificError(164, "Недопустимый пароль");
            AddSpecificError(165, "Недопустимый заводской номер ККМ");
            AddSpecificError(166, "Недопустимый РНМ");
            AddSpecificError(167, "Недопустимый ИНН");
            AddSpecificError(168, "ККМ не фискализирована");
            AddSpecificError(169, "Не задан заводской номер");
            AddSpecificError(170, "Нет отчетов");
            AddSpecificError(171, "Режим не активизирован");
            AddSpecificError(172, "Нет указанного чека в КЛ");
            AddSpecificError(173, "Нет больше записей КЛ");
            AddSpecificError(174, "Некорректный код или номер кода защиты ККМ");
            AddSpecificError(176, "Требуется выполнение общего гашения");
            AddSpecificError(177, "Команда не разрешена введенными кодами защиты ККМ");
            AddSpecificError(178, "Невозможна отмена скидки/надбавки");
            AddSpecificError(179, "Невозможно закрыть чек данным типом оплаты (в чеке присутствуют операции без контроля наличных)");
            AddSpecificError(180, "Неверный номер маршрута");
            AddSpecificError(181, "Неверный номер начальной зоны");
            AddSpecificError(182, "Неверный номер конечной зоны");
            AddSpecificError(183, "Неверный тип тарифа");
            AddSpecificError(184, "Неверный тариф");
            AddSpecificError(186, "Ошибка обмена с фискальным модулем");
            AddSpecificError(190, "Необходимо провести профилактические работы");
            AddSpecificError(200, "Нет устройства, обрабатывающего данную команду");
            AddSpecificError(201, "Нет связи с внешним устройством");
            AddSpecificError(202, "Неверное состояние пульта ТРК");
            AddSpecificError(203, "В чеке продажи топлива возможна только одна регистрация");
            AddSpecificError(204, "Неверный номер пульта ТРК");
            AddSpecificError(205, "Неверный делитель");
            AddSpecificError(206, "Недопустимое целевое устройство");
            AddSpecificError(207, "В ККМ произведено 20 активизаций");
            AddSpecificError(208, "Активизация данной ЭКЛЗ в составе данной ККМ невозможна");
            AddSpecificError(209, "Недопустимое межстрочие");
            AddSpecificError(210, "Ошибка обмена с ЭКЛЗ на уровне интерфейса I2C");
            AddSpecificError(211, "Ошибка формата передачи ЭКЛЗ");
            AddSpecificError(212, "Неверное состояние ЭКЛЗ");
            AddSpecificError(213, "Неисправимая ошибка ЭКЛЗ");
            AddSpecificError(214, "Авария крипто-процессора ЭКЛЗ");
            AddSpecificError(215, "Исчерпан временной ресурс ЭКЛЗ");
            AddSpecificError(216, "ЭКЛЗ переполнена");
            AddSpecificError(217, "В ЭКЛЗ переданы неверная дата или время");
            AddSpecificError(218, "В ЭКЛЗ нет запрошенных данных");
            AddSpecificError(219, "Переполнение ЭКЛЗ (итог чека)");
            AddSpecificError(254, "Снятие отчета прервалось");
            AddSpecificError(255, "Более одной клавиши ККМ нажаты одновременно");
            AddSpecificError(-1, "Неизвестная ошибка");

            _atolProtocol = new AtolProtocol(this, OPERATOR_PASSWD);
        }

        #endregion

        #region Внутренние методы

        private bool IsSlipMode()
        {
            return PrinterNumber != PrinterNumber.MainPrinter && _deviceModel == AtolModel.Felix3SK;
        }

        private DeviceParams GetDeviceParams()
        {
            DeviceParams devParams = new DeviceParams();
            switch (_deviceModel)
            {
                case AtolModel.FelixRF:
                case AtolModel.Felix02K:
                    devParams.MaxStringLen = 20;
                    devParams.StringLen = 20;
                    devParams.TapeWidth = 120 - 5;
                    devParams.BarcodeWidth = 1;
                    break;
                case AtolModel.Mercury140F:
                case AtolModel.Tornado:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 464 - 33;
                    devParams.BarcodeWidth = 3;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FelixRK:
                    devParams.MaxStringLen = 38;
                    devParams.StringLen = 32;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    break;
                case AtolModel.Felix3SK:
                    devParams.MaxStringLen = 38;
                    devParams.StringLen = 38;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    devParams.SlipLineSize = 38;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint02K:
                    devParams.MaxStringLen = 56;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 204;// 320 - 40;
                    devParams.BarcodeWidth = 1;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint03K:
                    devParams.MaxStringLen = 32;
                    devParams.StringLen = 32;
                    devParams.TapeWidth = 320 - 40;
                    devParams.BarcodeWidth = 2;
                    break;
                case AtolModel.FPrint5200K:
                case AtolModel.FPrint55K:
                    devParams.MaxStringLen = 36;
                    devParams.StringLen = 36;
                    devParams.TapeWidth = 360 - 40;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint22K:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 48;
                    devParams.TapeWidth = 390;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint88K:
                    devParams.MaxStringLen = 56;
                    devParams.StringLen = 42;
                    devParams.TapeWidth = 340;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                case AtolModel.FPrint22PTK:
                    devParams.MaxStringLen = 64;
                    devParams.StringLen = 48;
                    devParams.TapeWidth = 390;
                    devParams.BarcodeWidth = 2;
                    devParams.IsCutterSupported = true;
                    break;
                default:
                    devParams.MaxStringLen = 48;
                    devParams.StringLen = 40;
                    devParams.TapeWidth = 464 - 33;
                    devParams.BarcodeWidth = 3;
                    break;
            }

            return devParams;
        }

        // ожидание завершения команды
        private bool WaitForStateChange(ref byte nMode, ref byte nSubMode, ref byte nFlags)
        {
            byte nCurrentMode = 0;
            byte nCurrentSubMode = 0;
            bool bSuccess = false;

            do
            {
                try
                {
                    _atolProtocol.ExecuteCommand(0x45);
                    nCurrentMode = (byte)(_atolProtocol.Response[2] & 0x0F);
                    nCurrentSubMode = (byte)(_atolProtocol.Response[2] >> 4);
                    nFlags = _atolProtocol.Response[3];
                    bSuccess = (nCurrentMode != nMode) || (nCurrentSubMode != nSubMode);
                }
                catch (TimeoutException)
                {
                    bSuccess = false;
                }
                if (!bSuccess)
                    System.Threading.Thread.Sleep(500);
            }
            while (!bSuccess);

            nMode = nCurrentMode;
            nSubMode = nCurrentSubMode;
            return true;
        }

        // определение модели устройства и установка значения ширины ленты
        private void SetDeviceType()
        {
            _atolProtocol.ExecuteCommand(0xA5);
            _deviceModel = (AtolModel)_atolProtocol.Response[4];
        }

        private void SetTableField(byte table, ushort row, byte field, Action setFieldData)
        {
            // команда "Программирование таблицы"
            _atolProtocol.CreateCommand(0x50);

            // таблица
            _atolProtocol.AddByte(table);

            // ряд (сначала старший байт, потом младший)
            var rowBytes = BitConverter.GetBytes(row);

            _atolProtocol.AddByte(rowBytes[1]);
            _atolProtocol.AddByte(rowBytes[0]);

            // поле
            _atolProtocol.AddByte(field);

            // значение поля
            setFieldData();

            // выполняем команду
            _atolProtocol.Execute();
        }

        private void SetTableField(byte table, ushort row, byte field, string fieldValue, int? maxLength = null)
        {
            if (maxLength == null)
            {
                maxLength = GetDeviceParams().MaxStringLen;
            }

            SetTableField(table, row, field, () => _atolProtocol.AddString(fieldValue, maxLength.Value));
        }

        private void CleanupStoredImages()
        {
            _atolProtocol.CreateCommand(0x8A);
            _atolProtocol.AddByte(0);
            _atolProtocol.Execute();
        }

        // установка подвала документа
        private void SetFooter()
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);

            // запись строк подвала
            if (PrintFooter && DocumentFooter != null)
            {
                foreach (var item in DocumentFooter.Select((line, row) => new { line, row }).Take(5))
                {
                    SetTableField(6, (ushort)(1 + item.row), 1, item.line);
                }
            }

            // загрузка графического подвала
            if (PrintGraphicFooter && GraphicFooter != null)
            {
                if (!PrintGraphicHeader || GraphicHeader == null)
                {
                    // очистка массива картинок
                    CleanupStoredImages();
                }

                // загрузка картинки в память ФР
                var imageIndex = LoadImage(GraphicFooter);

                // запись строки клише с картинкой
                SetTableField(6, 1, 1, () =>
                {
                    var line = new byte[GetDeviceParams().MaxStringLen];

                    line[0] = 0x0A;

                    // номер картинки, загруженной в память
                    line[1] = (byte)imageIndex;

                    // смещение
                    line[2] = 0x00;
                    line[3] = (byte)((GetDeviceParams().TapeWidth - GraphicFooter.Width) / 2);

                    _atolProtocol.AddBytes(line);
                });
            }
        }

        // установка заголовка документа
        private void SetHeader()
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);

            // запись строк клише
            if (PrintHeader && DocumentHeader != null)
            {
                foreach (var item in DocumentHeader.Select((line, row) => new { line, row }).Take(8))
                {
                    SetTableField(6, (ushort)(6 + item.row), 1, item.line);
                }
            }

            // загрузка графического клише
            if (PrintGraphicHeader && GraphicHeader != null)
            {
                // установка волшебного флага в таблице для возможности загрузки графики;
                // таблица 2, ряд 1, поле 21, значение 30h
                SetTableField(2, 1, 21, () => _atolProtocol.AddBCD(48, 1));

                // очистка массива картинок
                CleanupStoredImages();

                // загрузка картинки в память ФР
                var imageIndex = LoadImage(GraphicHeader);

                // запись строки клише с картинкой
                SetTableField(6, 8, 1, () =>
                {
                    var line = new byte[GetDeviceParams().MaxStringLen];

                    line[0] = 0x0A;

                    // номер картинки, загруженной в память
                    line[1] = (byte)imageIndex;

                    // смещение
                    line[2] = 0x00;
                    line[3] = (byte)((GetDeviceParams().TapeWidth - GraphicHeader.Width) / 2);

                    _atolProtocol.AddBytes(line);
                });
            }
        }

        private void SetTapeWidth()
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);

            // установка кол-ва символов в строке
            SetTableField(2, 1, 55, () => _atolProtocol.AddBCD(GetDeviceParams().StringLen, 1));

            // установка сжатого по горизонтали шрифта
            SetTableField(2, 1, 56, () => _atolProtocol.AddBCD(2, 1));
        }

        // печать отчетов
        private void PrintReport(DocumentType docType)
        {
            byte nMode = 0;
            byte nSubMode = 0;
            byte nFlags = 0;
            switch (docType)
            {
                case DocumentType.XReport:
                    PrintXReport(1, out nMode, out nSubMode);
                    break;
                case DocumentType.ZReport:
                    nMode = 3;
                    nSubMode = 2;
                    _atolProtocol.SwitchToMode(3, MODE_PASSWD);
                    _atolProtocol.ExecuteCommand(0x5A);
                    break;
                case DocumentType.SectionsReport:
                    PrintXReport(2, out nMode, out nSubMode);
                    break;
                case DocumentType.FDOExchangeStateReport:
                    PrintXReport(9, out nMode, out nSubMode);
                    break;
            }

            if (!WaitForStateChange(ref nMode, ref nSubMode, ref nFlags))
                return;

            if (docType == DocumentType.ZReport)
            {
                if (nMode == 7 && nSubMode == 1)
                {
                    WaitForStateChange(ref nMode, ref nSubMode, ref nFlags);
                }

                if (nMode == 3 && nSubMode == 0)
                {
                    if ((nFlags & 0x01) == 0x01)
                        throw new DeviceErrorException(103);// нет бумаги
                    else
                    {
                        if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);// нет связи с принтером
                    }
                }
                else
                    throw new DeviceErrorException(254); // снятие отчета прервалось
            }
            else
            {
                if (nMode == 2 && nSubMode == 0)
                {
                    if ((nFlags & 0x01) == 0x01)
                        throw new DeviceErrorException(103);// нет бумаги
                    else
                    {
                        if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);// нет связи с принтером
                    }
                }
                else
                    throw new DeviceErrorException(254);// снятие отчета прервалось
            }
        }

        private void PrintXReport(int reportNumber, out byte nMode, out byte nSubMode)
        {
            nMode = 2;
            nSubMode = 2;
            _atolProtocol.SwitchToMode(2, MODE_PASSWD);
            _atolProtocol.CreateCommand(0x67);
            _atolProtocol.AddBCD(reportNumber, 1);
            _atolProtocol.Execute();
        }

        // отмена документа
        private void CancelDocument()
        {
            if (!DocOpened())
            {
                PrintStringInternal("ЧЕК АННУЛИРОВАН", FontStyle.Regular);
                _atolProtocol.ExecuteCommand(0x6C);
            }
            else
                _atolProtocol.ExecuteCommand(0x59);

            _docAmount = 0;
        }

        // получить состояние документа
        private bool DocOpened()
        {
            // запрос состояния
            _atolProtocol.ExecuteCommand(0x3F);
            return ((_atolProtocol.Response[18] & 1) == 1) && (_atolProtocol.Response[23] != 0);
        }

        // печать строки
        private void PrintStringInternal(string source, FontStyle style)
        {
            _atolProtocol.CreateCommand(0x87);
            _atolProtocol.AddByte(0); // режим проверки
            _atolProtocol.AddByte(1); // печать на чековой ленте
            _atolProtocol.AddByte(0); // шрифт

            // множитель по вертикали
            if (style == FontStyle.DoubleHeight || style == FontStyle.DoubleAll)
                _atolProtocol.AddByte(1);
            else
                _atolProtocol.AddByte(0);

            _atolProtocol.AddByte(0); // межстрочие                
            _atolProtocol.AddByte(0); // яркость
            _atolProtocol.AddByte(1); // режим ЧЛ
            _atolProtocol.AddByte(1); // режим КЛ
            _atolProtocol.AddByte(0); // форматирование
            _atolProtocol.AddByte(0); // резерв
            _atolProtocol.AddByte(0); // резерв
            //                Cmd.AddByte(0); // резерв
            _atolProtocol.AddString(source, source.Length, (style == FontStyle.DoubleWidth || style == FontStyle.DoubleAll)); // строка
            _atolProtocol.Execute();
        }

        // преобразование строки штрих-кода в массив байт для печати картинки
        private byte[] GetBarcodeData(ref string barcode, int nWidth)
        {
            // рассчет контрольной суммы            
            char[] barcodeChars = barcode.ToCharArray(0, 12);
            int nChecksum = 0;
            for (int i = 0; i < 12; i+=2)
            {
                nChecksum += Convert.ToInt32(barcodeChars[i].ToString());
                if (i + 1 < 12)
                    nChecksum += Convert.ToInt32(barcodeChars[i + 1].ToString()) * 3;
            }
            nChecksum = nChecksum % 10;
            if (nChecksum > 0)
                nChecksum = 10 - nChecksum;
            barcode += nChecksum.ToString();

            string sBarcodeData = "101";
            int numberSystem = Convert.ToInt32(barcode[0].ToString());
            for (int i = 1; i < 7; i++)
            {
                int value = Convert.ToInt32(barcode[i].ToString());
                sBarcodeData += EAN_CHARSET[value][EAN_PARITY[numberSystem][i - 1]];
            }
            sBarcodeData += "01010";
            for (int i = 7; i < 13; i++)
            {
                int value = Convert.ToInt32(barcode[i].ToString());
                sBarcodeData += EAN_CHARSET[value][RIGHT];
            }
            sBarcodeData += "101";
            sBarcodeData += "0";

            string sWideBarcodeData = "";
            foreach (char c in sBarcodeData)
            {
                for (int i = 0; i < nWidth; i++)
                    sWideBarcodeData += c;
            }

            byte[] barcodeData = new byte[12 * nWidth];
            for (int i = 0; i < 12 * nWidth; i++)
                barcodeData[i] = Convert.ToByte(sWideBarcodeData.Substring(i * 8, 8), 2);

            return barcodeData;
        }

        private void ExecuteDriverCommand(Action executeCommandAction)
        {
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
            try
            {
                if (!Active)
                {
                    ErrorCode = new ServerErrorCode(this, GeneralError.Inactive);
                    return;
                }

                executeCommandAction();
            }
            catch (TimeoutException)
            {
                ErrorCode = new ServerErrorCode(this, GeneralError.Timeout, _atolProtocol != null ? _atolProtocol.GetCommandDump() : string.Empty);
            }
            catch (DeviceErrorException E)
            {
                ErrorCode = new ServerErrorCode(this, E.ErrorCode, _atolProtocol != null ? GetSpecificDescription(E.ErrorCode) : string.Empty, _atolProtocol.GetCommandDump());
            }
            catch (Exception E)
            {
                ErrorCode = new ServerErrorCode(this, E, _atolProtocol != null ? _atolProtocol.GetCommandDump() : string.Empty);
            }
            finally
            {
                if (!ErrorCode.Succeeded)
                {
                    try
                    {
                        if (_atolProtocol.IsSlipMode)
                            _atolProtocol.IsSlipMode = false;
                        else if (DocOpened())
                            CancelDocument();
                    }
                    catch (DeviceErrorException)
                    {
                    }
                    catch (TimeoutException)
                    {
                    }
                }
            }
        }

        private byte[] GetImageBytes(System.Drawing.Bitmap image, out int stride)
        {
            image.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
            var rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            var bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            byte[] imageBytes = new byte[image.Height * bmpData.Stride];
            stride = bmpData.Stride;
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imageBytes, 0, bmpData.Height * stride);
                // инвертируем цвета
                if (image.Palette.Entries[0].Name == "ff000000")
                {
                    byte invertor = 0xFF;
                    int delta = stride * 8 - image.Width;
                    byte invertor2 = (byte)(invertor << delta);
                    for (int i = 0; i < imageBytes.Length; i++)
                    {
                        if( (i+1)%stride == 0)
                            imageBytes[i] = (byte)(imageBytes[i] ^ invertor2);
                        else
                            imageBytes[i] = (byte)(imageBytes[i] ^ invertor);
                    }
                }
                return imageBytes;
            }
            finally
            {
                image.UnlockBits(bmpData);
            }
        }

        /// <summary>
        /// Загрузка картинки в память устройства
        /// </summary>
        /// <param name="image">Картинка</param>
        /// <param name="index">Номер картинки</param>
        private int LoadImage(System.Drawing.Bitmap image)
        {
            // передача строк картинки
            int stride;
            var imageBytes = GetImageBytes(GraphicHeader, out stride);
            int bytesRead = 0;
            var lineBytes = new byte[stride];
            while (bytesRead < imageBytes.Length)
            {
                Array.Copy(imageBytes, bytesRead, lineBytes, 0, stride);
                _atolProtocol.CreateCommand(0x8B);
                _atolProtocol.AddBytes(lineBytes);
                _atolProtocol.Execute();
                bytesRead += stride;
            }
            // закрытие картинки
            _atolProtocol.ExecuteCommand(0x9E);

            // возвращаем номер новой картинки
            return _atolProtocol.Response[3];
        }

        /// <summary>
        /// Печать картинки из памяти устройства по номеру
        /// </summary>
        /// <param name="index">Номер картинки</param>
        private void PrintImage(int index)
        {
            _atolProtocol.CreateCommand(0x8D);
            _atolProtocol.AddByte(1); // печать на чековой ленте
            _atolProtocol.AddByte((byte)index); // номер картинки
            _atolProtocol.AddBCD(0, 2); // смещение
            _atolProtocol.Execute();
        }

        private void CloseSaleOrRefundDocument()
        {
            if (HasNonzeroRegistrations)
            {
                ExecuteDriverCommand(() =>
                {
                    _atolProtocol.CreateCommand(0x4A);
                    _atolProtocol.AddByte(0);
                    _atolProtocol.AddByte(1);
                    _atolProtocol.AddBCD(0, 5);
                    _atolProtocol.Execute();
                });
            }
            else
            {
                ExecuteDriverCommand(() =>
                {
                    _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                    _atolProtocol.ExecuteCommand(0x73);
                });
            }
        }

        private void SetCustomerPhoneOrEmail(string customerPhoneOrEmail)
        {
            ExecuteDriverCommand(() =>
            {
                _atolProtocol.CreateCommand(0xE8);

                // флаги (выводим реквизит на печать)
                _atolProtocol.AddByte(1);

                // количество блоков (всегда 1)
                _atolProtocol.AddByte(1);

                // номер блока (нулевой)
                _atolProtocol.AddByte(0);

                // данные реквизита (TLV)
                // тэг
                var tagBytes = BitConverter.GetBytes((ushort)1008);

                _atolProtocol.AddByte(tagBytes[0]);
                _atolProtocol.AddByte(tagBytes[1]);

                // длина
                var lengthToCopy = customerPhoneOrEmail.Length > 64 ? 64 : customerPhoneOrEmail.Length;
                var lengthToCopyBytes = BitConverter.GetBytes((ushort)lengthToCopy);

                _atolProtocol.AddByte(lengthToCopyBytes[0]);
                _atolProtocol.AddByte(lengthToCopyBytes[1]);

                // значение
                _atolProtocol.AddString(customerPhoneOrEmail, lengthToCopy);

                _atolProtocol.Execute();
            });
        }

        private void SetCashierField(byte field, string fieldValue, int? maxLength = null)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                return;
            }

            SetTableField(3, 30, field, fieldValue, maxLength);
        }

        private void SetCashierFields(string cashierName, string cashierInn)
        {
            _atolProtocol.SwitchToMode(4, MODE_PASSWD);

            SetCashierField(2, cashierName);
            SetCashierField(3, cashierInn, 12);
        }

        private void OpenDocument(DocumentType docType, string cashierName, string cashierInn)
        {
            // программируем имя и ИНН кассира
            SetCashierFields(cashierName, cashierInn);

            // открываем документ
            ExecuteDriverCommand(() =>
            {
                if (PrinterStatus.OpenedDocument)
                    CancelDocument();

                switch (docType)
                {
                    case DocumentType.Sale:
                    case DocumentType.Refund:
                        if (HasNonzeroRegistrations)
                        {
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            _atolProtocol.CreateCommand(0x92);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddBCD(docType == DocumentType.Sale ? 1 : 2, 1);
                            _atolProtocol.Execute();
                        }
                        break;
                    default:
                        if (IsSlipMode())
                        {
                            // переключение в режим регистрации
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            // установка фискальной станции
                            _atolProtocol.IsSlipMode = true;
                        }
                        break;
                }

                _currDocType = docType;
                _docAmount = 0;
            });
        }

        #endregion

        #region Виртуальные методы

        protected override void OnAfterActivate()
        {
            Port.ReadTimeout = READ_TIMEOUT;
            Port.WriteTimeout = WRITE_TIMEOUT;

            ExecuteDriverCommand(delegate()
           {
               // определяем модель устройства
               SetDeviceType();
               // установка длины строки (только для Феликс-3СК)
               if (_deviceModel == AtolModel.Felix3SK)
                   SetTapeWidth();
               SetHeader();
               SetFooter();
           });
        }

        protected override void OnOpenDocument(DocumentType docType, string cashierName) => OpenDocument(docType, cashierName, null);

        protected override void OnOpenDocument(DocumentType docType, string cashierName, string cashierInn, string customerPhoneOrEmail)
        {
            // открываем документ
            OpenDocument(docType, cashierName, cashierInn);

            // теперь записываем реквизит "Адрес покупателя"
            SetCustomerPhoneOrEmail(customerPhoneOrEmail);
        }

        protected override void OnCloseDocument(bool cutPaper)
        {            
            if (_currDocType == DocumentType.Sale || _currDocType == DocumentType.Refund)
            {
                CloseSaleOrRefundDocument();
            }
            else
            {
                ExecuteDriverCommand(() =>
                {
                    switch (_currDocType)
                    {
                        case DocumentType.PayingIn:
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            _atolProtocol.CreateCommand(0x49);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddBCD(_docAmount, 5);
                            _atolProtocol.Execute();
                            break;
                        case DocumentType.PayingOut:
                            _atolProtocol.SwitchToMode(1, MODE_PASSWD);
                            _atolProtocol.CreateCommand(0x4F);
                            _atolProtocol.AddByte(0);
                            _atolProtocol.AddBCD(_docAmount, 5);
                            _atolProtocol.Execute();
                            break;
                        case DocumentType.Other:
                            if (IsSlipMode())
                            {
                                // возврат документа
                                _atolProtocol.CreateCommand(0x8F);
                                _atolProtocol.AddByte(2);
                                _atolProtocol.AddBytes(new byte[] { 0x1B, 0x0C, 3 });
                                _atolProtocol.Execute();

                                // установка фискальной станции
                                _atolProtocol.IsSlipMode = false;
                            }
                            else
                            {
                                // печать подвала
                                _atolProtocol.SwitchToMode(2, MODE_PASSWD);
                                _atolProtocol.ExecuteCommand(0x73);

                                // промотка и обрезка
                                if (cutPaper && _deviceModel == AtolModel.FPrint22K)
                                {
                                    _atolProtocol.CreateCommand(0x75);
                                    _atolProtocol.AddByte(1);
                                    _atolProtocol.Execute();
                                }
                            }
                            break;
                        case DocumentType.XReport:
                        case DocumentType.ZReport:
                        case DocumentType.SectionsReport:
                        case DocumentType.FDOExchangeStateReport:
                            PrintReport(_currDocType);
                            break;
                    }
                    _docAmount = 0;
                });
            }
        }

        protected override void OnOpenDrawer()
        {
            ExecuteDriverCommand(() => _atolProtocol.ExecuteCommand(0x80));
        }

        protected override void OnPrintString(string source, FontStyle style)
        {
            ExecuteDriverCommand(delegate()
            {
                try
                {
                    PrintStringInternal(source, style);
                }
                catch (DeviceErrorException E)
                {
                    if (!IsSlipMode() || E.ErrorCode != 103)
                        throw;

                    _atolProtocol.CreateCommand(0x8F);
                    _atolProtocol.AddByte(2);
                    _atolProtocol.AddBytes(new byte[] { 0x1B, 0x0C, 3 });
                    _atolProtocol.Execute();

                    // проверка состояния бумаги
                    do
                    {
                        if (PrinterStatus.PaperOut == PaperOutStatus.Present)
                            break;
                        System.Threading.Thread.Sleep(1000);
                    }
                    while (true);
                    PrintStringInternal(source, style);
                }
            });
        }

        protected override void OnPrintBarcode(string barcode, AlignOptions align,
            bool readable)
        {
            ExecuteDriverCommand(delegate()
            {
                byte[] nRaster = GetBarcodeData(ref barcode, GetDeviceParams().BarcodeWidth);
                _atolProtocol.CreateCommand(0x8E);
                _atolProtocol.AddByte(1);     // печатать на чековой ленте
                _atolProtocol.AddBCD(25 * GetDeviceParams().BarcodeWidth, 2);  // количество повторов строки
                switch (align)
                {
                    case AlignOptions.Left:
                        _atolProtocol.AddBCD(0, 2);   // смещение
                        break;
                    case AlignOptions.Center:
                        _atolProtocol.AddBCD((GetDeviceParams().TapeWidth - 95 * GetDeviceParams().BarcodeWidth) / 2 - 1, 2);   // смещение
                        break;
                    case AlignOptions.Right:
                        _atolProtocol.AddBCD(GetDeviceParams().TapeWidth - 95 * GetDeviceParams().BarcodeWidth - 1, 2);   // смещение
                        break;
                }

                _atolProtocol.AddBytes(nRaster);// растр
                _atolProtocol.Execute();

                if (readable)
                {
                    string barcodeString = "";
                    switch (align)
                    {
                        case AlignOptions.Left:
                            barcodeString = new string(' ', 2 * GetDeviceParams().BarcodeWidth);
                            break;
                        case AlignOptions.Center:
                            barcodeString = new string(' ', (GetDeviceParams().StringLen - barcode.Length) / 2);
                            break;
                        case AlignOptions.Right:
                            barcodeString = new string(' ', GetDeviceParams().StringLen - barcode.Length - 2 * GetDeviceParams().BarcodeWidth);
                            break;
                    }
                    barcodeString += barcode;
                    PrintStringInternal(barcodeString, FontStyle.Regular);
                }
            });
        }

        protected override void OnPrintImage(System.Drawing.Bitmap image, AlignOptions align)
        {
            ExecuteDriverCommand(delegate() 
            {
                int index = LoadImage(image);
                PrintImage(index);
            });
        }

        protected override void OnRegistration(string commentary, uint quantity, uint amount, byte section)
        {
            if (!HasNonzeroRegistrations)
            {
                return;
            }

            ExecuteDriverCommand(() =>
            {
                switch (_currDocType)
                {
                    case DocumentType.Sale:
                        _atolProtocol.CreateCommand(0x52);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.AddBCD(amount, 5);
                        _atolProtocol.AddBCD(quantity, 5);
                        _atolProtocol.AddBCD(section, 1);
                        _atolProtocol.Execute();
                        break;
                    case DocumentType.Refund:
                        _atolProtocol.CreateCommand(0x57);
                        _atolProtocol.AddByte(2);
                        _atolProtocol.AddBCD(amount, 5);
                        _atolProtocol.AddBCD(quantity, 5);
                        _atolProtocol.Execute();
                        break;
                    default:
                        break;
                }
            });
        }

        protected override void OnRegistration(string positionName, uint quantity, uint price, uint amount, byte section, byte vatRateId)
        {
            if (!HasNonzeroRegistrations)
            {
                return;
            }

            ExecuteDriverCommand(() =>
            {
                _atolProtocol.CreateCommand(0xE6);
                
                // флаги по умолчанию (выполнить операцию, контроль наличности)
                _atolProtocol.AddByte(0);

                // наименование позиции
                _atolProtocol.AddString(positionName.PadRight(64, (char)0x20), 64);

                // цена
                _atolProtocol.AddBCD(price, 6);

                // количество
                _atolProtocol.AddBCD(quantity, 5);

                // тип скидки (не используется)
                _atolProtocol.AddByte(0);

                // знак скидки (не используется)
                _atolProtocol.AddByte(0);

                // размер скидки (не используется)
                _atolProtocol.AddBytes(new byte[6]);

                // налог
                _atolProtocol.AddByte(vatRateId);

                // секция
                _atolProtocol.AddBCD(section, 1);

                // штрихкод (не используется)
                _atolProtocol.AddBytes(new byte[16]);

                // резерв
                _atolProtocol.AddByte(0);

                _atolProtocol.Execute();
            });
        }

        protected override void OnRegistration(string positionName, uint quantity, uint price, uint amount, byte section, byte vatRateId, byte fiscalItemType)
        {
            if (!HasNonzeroRegistrations)
            {
                return;
            }

            ExecuteDriverCommand(() =>
            {
                _atolProtocol.CreateCommand(0xEB);

                // флаги по умолчанию (выполнить операцию, контроль наличности)
                _atolProtocol.AddByte(0);

                // цена
                _atolProtocol.AddBCD(price, 7);

                // количество
                _atolProtocol.AddBCD(quantity, 5);

                // стоимость позиции
                _atolProtocol.AddBCD(amount, 7);

                // налог
                _atolProtocol.AddByte(vatRateId);

                // сумма налога (ККТ считает самостоятельно)
                _atolProtocol.AddBCD(0, 7);

                // секция
                _atolProtocol.AddBCD(section, 1);

                // признак предмета расчета
                _atolProtocol.AddByte(fiscalItemType);

                // признак способа расчета (всегда полный расчет)
                _atolProtocol.AddByte(4);

                // знак скидки (не используется)
                _atolProtocol.AddByte(0);

                // информация о скидке (не используется)
                _atolProtocol.AddBCD(0, 7);

                // здесь либо ошибка в спецификации, либо в прошивке;
                // если не пропустить эти два байта, то первые два символа в наименовании позиции обрезаются
                _atolProtocol.AddByte(0);
                _atolProtocol.AddByte(0);

                // наименование позиции
                _atolProtocol.AddString(positionName, 128);

                _atolProtocol.Execute();
            });
        }

        protected override void OnTrimDocumentAmount(uint registrationsAmount)
        {
            // посчитаем разницу между суммой регистраций и суммой документа по данным ФР
            var amountDelta = (long)Status.DocumentAmount - registrationsAmount;
            if (amountDelta > 0 && amountDelta < 100)
            {
                // сумма чека в ФР больше, чем сумма платежей по чеку, и разница не превышает 1 рубль;
                // считаем это ошибкой округления и делаем скидку
                ExecuteDriverCommand(() =>
                {
                    _atolProtocol.CreateCommand(0x43);

                    // флаги по умолчанию (выполнить операцию)
                    _atolProtocol.AddByte(0);

                    // область применения (на весь чек)
                    _atolProtocol.AddByte(0);

                    // тип (суммовая скидка)
                    _atolProtocol.AddByte(1);

                    // знак (скидка)
                    _atolProtocol.AddByte(0);

                    // размер скидки
                    _atolProtocol.AddBCD(amountDelta, 5);

                    _atolProtocol.Execute();
                });
            }
        }

        protected override void OnPayment(uint amount, FiscalPaymentType paymentType)
        {
            ExecuteDriverCommand(() =>
            {
                if (HasNonzeroRegistrations && (_currDocType == DocumentType.Sale || _currDocType == DocumentType.Refund))
                {
                    _atolProtocol.CreateCommand(0x99);
                    _atolProtocol.AddByte(0);
                    switch (paymentType)
                    {
                        case FiscalPaymentType.Cash:
                            _atolProtocol.AddBCD(1, 1);
                            break;
                        case FiscalPaymentType.Card:
                            _atolProtocol.AddBCD(2, 1);
                            break;
                        case FiscalPaymentType.Other1:
                            _atolProtocol.AddBCD(3, 1);
                            break;
                        case FiscalPaymentType.Other2:
                            _atolProtocol.AddBCD(4, 1);
                            break;
                        case FiscalPaymentType.Other3:
                            _atolProtocol.AddBCD(5, 1);
                            break;
                    }
                    _atolProtocol.AddBCD(amount, 5);
                    _atolProtocol.Execute();
                    _docAmount += amount;
                }
                else
                    _docAmount += amount;
            });
        }

        protected override void OnCash(uint amount)
        {
            ExecuteDriverCommand(delegate()
            {
                _docAmount += amount;
            });
        }

        #endregion

        #region События

        public override event EventHandler<FiscalBreakEventArgs> FiscalBreak;

        public override event EventHandler<PrinterBreakEventArgs> PrinterBreak;

        #endregion

        #region Свойства

        public override DateTime CurrentTimestamp
        {
            get
            {
                DateTime dateTime = DateTime.Now;
                ExecuteDriverCommand(delegate()
                {
                    // запрос состояния
                    _atolProtocol.ExecuteCommand(0x3F);
                    dateTime = new DateTime((int)_atolProtocol.GetFromBCD(4, 1) + 2000,
                        (int)_atolProtocol.GetFromBCD(5, 1),
                        (int)_atolProtocol.GetFromBCD(6, 1),
                        (int)_atolProtocol.GetFromBCD(7, 1),
                        (int)_atolProtocol.GetFromBCD(8, 1),
                        (int)_atolProtocol.GetFromBCD(9, 1));
                });
                return dateTime;
            }
            set
            {
                ExecuteDriverCommand(delegate()
                {
                    // программирование даты
                    _atolProtocol.CreateCommand(0x64);
                    _atolProtocol.AddBCD(value.Day, 1);
                    _atolProtocol.AddBCD(value.Month, 1);
                    if (value.Year < 2000)
                        _atolProtocol.AddBCD(value.Year - 1900, 1);
                    else
                        _atolProtocol.AddBCD(value.Year - 2000, 1);
                    _atolProtocol.Execute();

                    // программирование времени
                    _atolProtocol.CreateCommand(0x4B);
                    _atolProtocol.AddBCD(value.Hour, 1);
                    _atolProtocol.AddBCD(value.Minute, 1);
                    _atolProtocol.AddBCD(value.Second, 1);
                    _atolProtocol.Execute();
                    _atolProtocol.Execute();
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
                    // запрос состояния
                    _atolProtocol.ExecuteCommand(0x3F);
                    long sNo = _atolProtocol.GetFromBCD(11, 4);
                    serialNo = sNo.ToString();
                });
                return new FiscalDeviceInfo(DeviceNames.ecrTypeAtol, serialNo);
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
                    // запрос состояния
                    _atolProtocol.ExecuteCommand(0x3F);

                    bOpenedShift = (_atolProtocol.Response[10] & 2) == 2;
                    bFiscalized = (_atolProtocol.Response[10] & 1) == 1;
                    bLocked = ((_atolProtocol.Response[18] & 0x0F) == 5) && ((_atolProtocol.Response[18] >> 4) == 1);
                    bOverShift = false;

                    // наличность в ДЯ
                    _atolProtocol.ExecuteCommand(0x4D);
                    nCashInDrawer = (int)_atolProtocol.GetFromBCD(2, 7);

                    // сумма документа
                    if (_currDocType == DocumentType.PayingIn || _currDocType == DocumentType.PayingOut)
                        nDocAmount = (int)_docAmount;
                    else
                    {
                        _atolProtocol.CreateCommand(0x91);
                        _atolProtocol.AddByte(0x14);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.AddByte(0);
                        _atolProtocol.Execute();
                        nDocAmount = (int)_atolProtocol.GetFromBCD(3, 5);
                    }
                });
                return new FiscalStatusFlags(bOpenedShift, bOverShift, bLocked, bFiscalized, (ulong)nDocAmount, (ulong)nCashInDrawer);
            }
        }

        public override PrintableDeviceInfo PrinterInfo
        {
            get
            {
                return new PrintableDeviceInfo(new PrintableTapeWidth(GetDeviceParams().StringLen, GetDeviceParams().SlipLineSize), true);
            }
        }

        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                var printing = false;
                var paperOutStatus = PaperOutStatus.OutPassive;
                var drawerOpened = false;
                var docOpened = false;

                ExecuteDriverCommand(delegate()
                {
                    // запрос кода состояния
                    _atolProtocol.ExecuteCommand(0x45);

                    if ((_atolProtocol.Response[3] & 0x01) == 0)
                        paperOutStatus = PaperOutStatus.Present;
                    else
                        paperOutStatus = PaperOutStatus.OutPassive;

                    // запрос состояния
                    _atolProtocol.ExecuteCommand(0x3F);

                    drawerOpened = (_atolProtocol.Response[10] & 4) == 4;

                    // датчик бумаги может сигнализировать о том, что бумага есть, но крышка может быть открыта, или плохо закрыта;
                    // это приведет к тому, что любая команда печати вернет ошибку 103 (отсутствие бумаги), а логика печати
                    // отсутствие бумаги не обнаружит;
                    // поэтому в обязательном порядке проверяем датчик крышки - если она открыта, считаем, что бумаги нет;
                    // если она закрыта, ориентируемся на то значение OutOfPaper, которое было установлено ранее
                    if ((_atolProtocol.Response[10] & 32) == 32)
                    {
                        paperOutStatus = PaperOutStatus.OutPassive;
                    }

                    printing = (_atolProtocol.Response[18] >> 4) != 0;

                    if ((_atolProtocol.Response[18] & 1) == 1)
                        docOpened = _atolProtocol.Response[23] != 0;
                });

                return new PrinterStatusFlags(printing, paperOutStatus, docOpened, drawerOpened);
            }
        }

        #endregion

        #region Методы

        public override void FiscalReport(FiscalReportType reportType, bool full, params object[] reportParams)
        {
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);

                switch (reportType)
                {
                    case FiscalReportType.ByDates:
                        _atolProtocol.CreateCommand(0x65);
                        if (full)
                            _atolProtocol.AddByte(1);
                        else
                            _atolProtocol.AddByte(0);

                        DateTime dtParam = (DateTime)(reportParams[0]);
                        _atolProtocol.AddBCD(dtParam.Day, 1);
                        _atolProtocol.AddBCD(dtParam.Month, 1);
                        if (dtParam.Year < 2000)
                            _atolProtocol.AddBCD(dtParam.Year - 1900, 1);
                        else
                            _atolProtocol.AddBCD(dtParam.Year - 2000, 1);

                        dtParam = (DateTime)(reportParams[1]);
                        _atolProtocol.AddBCD(dtParam.Day, 1);
                        _atolProtocol.AddBCD(dtParam.Month, 1);
                        if (dtParam.Year < 2000)
                            _atolProtocol.AddBCD(dtParam.Year - 1900, 1);
                        else
                            _atolProtocol.AddBCD(dtParam.Year - 2000, 1);
                        _atolProtocol.Execute();
                        break;
                    case FiscalReportType.ByShifts:
                        _atolProtocol.CreateCommand(0x66);
                        if (full)
                            _atolProtocol.AddByte(1);
                        else
                            _atolProtocol.AddByte(0);

                        _atolProtocol.AddBCD((long)(int)(reportParams[0]), 2);
                        _atolProtocol.AddBCD((long)(int)(reportParams[1]), 2);
                        _atolProtocol.Execute();
                        break;
                }

                byte nMode = 5;
                byte nSubMode = 2;
                byte nFlags = 0;

                if (WaitForStateChange(ref nMode, ref nSubMode, ref nFlags))
                {
                    if ((nMode == 5) && (nSubMode == 0))
                    {
                        if ((nFlags & 0x01) == 0x01)
                            throw new DeviceErrorException(103);            // нет бумаги
                        else if ((nFlags & 0x02) == 0x02)
                            throw new DeviceErrorException(104);            // нет связи с принтером
                    }
                    else
                        throw new DeviceErrorException(254);                // снятие отчета прервалось

                }
            });
        }

        public override void Fiscalization(int newPassword, long registrationNumber, long taxPayerNumber)
        {
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);
                _atolProtocol.CreateCommand(0x62);
                _atolProtocol.AddBCD(registrationNumber, 5);
                _atolProtocol.AddBCD(taxPayerNumber, 6);
                _atolProtocol.AddBCD(newPassword, 4);
                _atolProtocol.Execute();
            });
        }

        public override void GetLifetime(out DateTime firstDate, out DateTime lastDate, out int firstShift, out int lastShift)
        {
            DateTime _firstDate = DateTime.Now;
            DateTime _lastDate = DateTime.Now;
            int _firstShift = 1;
            int _lastShift = 2000;
            ExecuteDriverCommand(delegate()
            {
                _atolProtocol.SwitchToMode(5, TaxerPassword);

                _atolProtocol.ExecuteCommand(0x63);

                int nDay = (int)_atolProtocol.GetFromBCD(4, 1);
                int nMonth = (int)_atolProtocol.GetFromBCD(5, 1);
                int nYear = (int)_atolProtocol.GetFromBCD(6, 1);
                _firstDate = new DateTime(nYear < 97 ? nYear += 1900 : nYear += 2000, nMonth, nDay);

                nDay = (int)_atolProtocol.GetFromBCD(7, 1);
                nMonth = (int)_atolProtocol.GetFromBCD(8, 1);
                nYear = (int)_atolProtocol.GetFromBCD(9, 1);
                _lastDate = new DateTime(nYear < 97 ? nYear += 1900 : nYear += 2000, nMonth, nDay);

                _firstShift = (int)_atolProtocol.GetFromBCD(10, 2);
                _lastShift = (int)_atolProtocol.GetFromBCD(12, 2);
            });

            firstDate = _firstDate;
            lastDate = _lastDate;
            firstShift = _firstShift;
            lastShift = _lastShift;
        }

        #endregion

        #region Рализация ICommunicationPortProvider

        public EasyCommunicationPort GetCommunicationPort()
        {
            return Port;
        }

        #endregion
    }
}
