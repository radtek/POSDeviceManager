using System;
using System.Text;
using DevicesBase;
using DevicesBase.Communicators;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace TigerE
{
    /// <summary>
    /// Драйвер весов Mettler Toledo Tiger E
    /// </summary>
    [Scale("Tiger E")]
    public class TigerEScales : CustomSerialDevice, IScaleDevice
    {
        #region Константы

        private const int MaxAttempts = 10;
        private const int RawDataSize = 18;
        private const int ByteTimeout = 100;

        #endregion

        #region Поля

        private string _connStr;

        #endregion;

        #region Перегрузка методов

        protected override void OnAfterActivate()
        {
            Port.StopBits = StopBits.One;
            Port.DataBits = 8;
            Port.Parity = Parity.Even;
            Port.DsrFlow = false;
            Port.RtsEnable = false;
            Port.ReadTimeout = ByteTimeout * RawDataSize;
            Port.WriteTimeout = ByteTimeout;
        }

        #endregion

        #region Реализация IScaleDevice

        public string ConnectionString
        {
            get
            {
                return _connStr;
            }
            set
            {
                _connStr = value;

                // разбираем строку подключения
                ConnStrHelper connStrHelper = new ConnStrHelper(_connStr);

                // поддерживается обмен только по RS-232
                if (string.Compare(connStrHelper[1], "rs", true) != 0)
                    throw new InvalidOperationException("Весы поддерживают обмен только по интерфейсу RS-232");

                // инициализируем параметры связи
                PortName = connStrHelper[2];
                Baud = Convert.ToInt32(connStrHelper[3]);
            }
        }

        public void Upload(string xmlData)
        {
            // не поддерживается
        }

        public int Weight
        {
            get 
            {
                var attemptNumber = 0;
                do
                {
                    attemptNumber++;
                    try
                    {
                        // отправялем запрос веса устройству
                        Port.DiscardBuffers();
                        Port.WriteByte(0x03);

                        // читаем ответ от устройства
                        var rawData = new byte[RawDataSize];
                        var bytesRead = Port.Read(rawData, 0, RawDataSize);
                        if (bytesRead != RawDataSize)
                            throw new InvalidOperationException("Ошибка в формате данных");

                        // данные веса идут в обратном порядке, инвертируем часть буфера
                        // ответа
                        Array.Reverse(rawData, 0, 6);
                        // конвертируем часть буфера ответа в строку
                        var weight = Encoding.Default.GetString(rawData, 0, 6);
                        // возвращаем показания веса
                        return int.Parse(weight);
                    }
                    catch (TimeoutException)
                    {
                        if (attemptNumber == MaxAttempts)
                            throw;
                    }
                }
                while (attemptNumber < MaxAttempts);
                return -1;
            }
        }

        #endregion
    }
}
