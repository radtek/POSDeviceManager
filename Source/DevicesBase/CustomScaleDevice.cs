using System;
using System.Collections.Generic;
using System.Xml;
using DevicesBase.Communicators;
using DevicesBase.Helpers;
using DevicesCommon;
using DevicesCommon.Helpers;

namespace DevicesBase
{
    /// <summary>
    /// Тип рекламной информации
    /// </summary>
    public enum AdvertisingType
    {
        /// <summary>
        /// Наименование магазина
        /// </summary>
        ShopName,

        /// <summary>
        /// Бегущая строка
        /// </summary>
        CreepingLine
    }

    /// <summary>
    /// Базовый класс для весов
    /// </summary>
    public abstract class CustomScaleDevice : CustomDevice, IScaleDevice
    {
        private string _connectionString;
        private ICommunicator _communicator;
        private Dictionary<string, int> _dataRecordsCount;

        /// <summary>
        /// Создает устройство-весы
        /// </summary>
        protected CustomScaleDevice()
            : base()
        {
            _dataRecordsCount = new Dictionary<string, int>();
        }

        /// <summary>
        /// Возвращает интерфейс коммуникатора в зависимости от строки подключения
        /// </summary>
        private ICommunicator GetCommunicator()
        {
            if (_communicator == null)
            {
                // если коммуникатор еще не создан
                ConnStrHelper connStrHelper = new ConnStrHelper(_connectionString);
                
                // определяем протокол связи
                switch (connStrHelper[1])
                {
                    case "rs":
                        _communicator = new SerialCommunicator(connStrHelper[2],
                            Convert.ToInt32(connStrHelper[3]));
                        break;
                    case "tcp":
                        _communicator = new TcpCommunicator(connStrHelper[2],
                            Convert.ToInt32(connStrHelper[3]));
                        break;
                    default:
                        throw new InvalidOperationException(
                            string.Format("Протокол {0} не поддерживается.",
                            connStrHelper[1]));
                }

                return _communicator;
            }
            else
                // ссылка на ранее созданный интерфейс коммуникатора
                return _communicator;
        }

        #region Для реализации в классах-потомках

        /// <summary>
        /// Доп. инициализация коммуниктора после установки соединения
        /// </summary>
        /// <param name="communicator">Коммуникатор</param>
        protected virtual void OnOpen(ICommunicator communicator)
        {
        }

        /// <summary>
        /// Возвращает текущие показания веса
        /// </summary>
        /// <param name="communicator">Коммуникатор, подключенный к устройству</param>
        protected virtual int GetWeight(ICommunicator communicator)
        {
            return 0;
        }

        /// <summary>
        /// Выгрузка товара
        /// </summary>
        /// <param name="articleName">Наименование товара</param>
        /// <param name="articleCode">Код (артикул) товара</param>
        /// <param name="PLU">Номер товара в памяти весов</param>
        /// <param name="price">Цена за единицу взвешивания в копейках</param>
        /// <param name="units">Количество грамм в единице взвешивания</param>
        /// <param name="box">Вес тары в граммах</param>
        /// <param name="shelfLife">Срок годности в днях</param>
        /// <param name="message">Номер сообщения (экстра-текста)</param>
        protected virtual bool OnArticleUpload(string articleName, int articleCode, int PLU,
            int price, int units, int box, int shelfLife, int message)
        {
            return true;
        }

        /// <summary>
        /// Выгрузка сообщения (экстра-текста)
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        /// <param name="messageNumber">Номер сообщения в памяти весов</param>
        protected virtual bool OnMessageUpload(string messageText, int messageNumber)
        {
            return true;
        }

        /// <summary>
        /// Выгрузка рекламной информации
        /// </summary>
        /// <param name="advertisingText">Текст рекламной информации</param>
        /// <param name="advertisingType">Тип рекламной информации <see cref="AdvertisingType"/></param>
        protected virtual bool OnAdvertisingUpload(string advertisingText,
            AdvertisingType advertisingType)
        {
            return true;
        }

        #endregion

        #region Выгрузка данных

        /// <summary>
        /// Возвращает значение атрибута (Int32)
        /// </summary>
        /// <param name="element">Xml-элемент</param>
        /// <param name="attributeName">Имя атрибута</param>
        /// <param name="required">Обязательный или нет</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        private int GetInt32AttributeValue(XmlElement element, string attributeName,
            bool required, int defaultValue)
        {
            if (!element.HasAttribute(attributeName))
            {
                if (required)
                    throw new Exception(string.Format("Не задан обязательный атрибут \"{0}\"", attributeName));
                else
                    return defaultValue;
            }

            return Convert.ToInt32(element.GetAttribute(attributeName));
        }

        /// <summary>
        /// Выгрузка товаров
        /// </summary>
        /// <param name="articlesRoot">xml-элемент "Товары"</param>
        /// <param name="startIndex">Индекс записи, с которой начинать выгрузку</param>
        private bool UploadArticles(XmlElement articlesRoot, int startIndex)
        {
            if (startIndex < 0)
                return true;

            int itemsProceeded = 0;
            bool uploadResult = false;

            for (int i = startIndex - 1; i < articlesRoot.ChildNodes.Count; i++)
            {
                // очередной товар
                XmlElement article = (XmlElement)articlesRoot.ChildNodes[i];

                // выгрузка 
                uploadResult = OnArticleUpload(
                    article.InnerText,
                    GetInt32AttributeValue(article, "code", true, 0),
                    GetInt32AttributeValue(article, "plu", true, 0),
                    GetInt32AttributeValue(article, "price", false, 0),
                    GetInt32AttributeValue(article, "units", false, 1000),
                    GetInt32AttributeValue(article, "box", false, 0),
                    GetInt32AttributeValue(article, "shelfLife", false, 0),
                    GetInt32AttributeValue(article, "message", false, 0));
                // увеличиваем количество выгруженных записей
                itemsProceeded++;
            }

            // устанавливаем итоговое число обработанных записей
            _dataRecordsCount["articles"] += itemsProceeded;
            return uploadResult;
        }

        /// <summary>
        /// Выгрузка сообщений 
        /// </summary>
        /// <param name="messagesRoot">xml-элемент "сообщения"</param>
        /// <param name="startIndex">Индекс записи, с которой начинать выгрузку</param>
        private bool UploadMessages(XmlElement messagesRoot, int startIndex)
        {
            if (startIndex < 0)
                return true;

            int itemsProceeded = 0;
            bool uploadResult = false;

            for (int i = startIndex - 1; i < messagesRoot.ChildNodes.Count; i++)
            {
                // очередное сообщение
                XmlElement message = (XmlElement)messagesRoot.ChildNodes[i];

                // выгрузка 
                uploadResult = OnMessageUpload(
                    message.InnerText,
                    GetInt32AttributeValue(message, "number", false, 1));
                // увеличиваем количество выгруженных записей
                itemsProceeded++;
            }

            // устанавливаем итоговое число обработанных записей
            _dataRecordsCount["messages"] += itemsProceeded;
            return uploadResult;
        }

        /// <summary>
        /// Выгрузка рекламной информации
        /// </summary>
        /// <param name="advertisingsRoot"></param>
        /// <param name="startIndex">Индекс записи, с которой начинать выгрузку</param>
        private bool UploadAdvertisings(XmlElement advertisingsRoot, int startIndex)
        {
            if (startIndex < 0)
                return true;

            int itemsProceeded = 0;
            bool uploadResult = false;

            for (int i = startIndex - 1; i < advertisingsRoot.ChildNodes.Count; i++)
            {
                // очередная строка рекламной информации
                XmlElement advertising = (XmlElement)advertisingsRoot.ChildNodes[i];

                // выгрузка 
                uploadResult = OnAdvertisingUpload(
                    advertising.InnerText,
                    advertising.GetAttribute("type") == "shopName" ? AdvertisingType.ShopName :
                        AdvertisingType.CreepingLine);
                // увеличиваем количество выгруженных записей
                itemsProceeded++;
            }

            // устанавливаем итоговое число обработанных записей
            _dataRecordsCount["messages"] += itemsProceeded;
            return uploadResult;
        }

        #endregion

        #region Реализация IScaleDevice

        /// <summary>
        /// Строка подключения к весам
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set 
            { 
                // изменилась строка подключения - нужно пересоздать коммуникатор
                _communicator = null;
                _connectionString = value; 
            }
        }

        /// <summary>
        /// Выгрузка данных в весы
        /// </summary>
        /// <param name="xmlData">Данные</param>
        public void Upload(string xmlData)
        {
            try
            {
                // определяем коммуникатор
                using (ICommunicator communicator = GetCommunicator())
                {
                    // устанавливаем соединение с устройством
                    communicator.Open();
                    OnOpen(communicator);

                    // готовим таблицу счетчиков к выгрузке
                    _dataRecordsCount.Clear();
                    _dataRecordsCount.Add("articles", 0);
                    _dataRecordsCount.Add("messages", 0);
                    _dataRecordsCount.Add("advertisings", 0);

                    // создаем xml-документ
                    XmlDocument uploadData = new XmlDocument();
                    uploadData.LoadXml(xmlData);
                    if (string.Compare(uploadData.DocumentElement.Name, "scale", true) != 0)
                        throw new CommunicationException("Данные не предназначены для выгрузки в весы");

                    // для всех наборов данных
                    foreach (XmlElement uploadTag in uploadData.DocumentElement.ChildNodes)
                    {
                        // порядковый номер элемента, с которого начинать выгрузку данных
                        int startIndex = GetInt32AttributeValue(uploadTag, "startIndex", false, 1);

                        // поиск набора данных
                        if (string.Compare(uploadTag.Name, "articles") == 0)
                        {
                            // товары
                            _dataRecordsCount["articles"] = startIndex;
                            if (!UploadArticles(uploadTag, startIndex))
                                break;
                            continue;
                        }

                        if (string.Compare(uploadTag.Name, "messages") == 0)
                        {
                            // сообщения
                            _dataRecordsCount["messages"] = startIndex;
                            if (!UploadMessages(uploadTag, startIndex))
                                break;
                            continue;
                        }

                        if (string.Compare(uploadTag.Name, "advertisings") == 0)
                        {
                            // реклама
                            _dataRecordsCount["advertisings"] = startIndex;
                            if (!UploadAdvertisings(uploadTag, startIndex))
                                break;
                        }
                    }
                }
            }
            catch (CommunicationException e)
            {
                ErrorCode = new ServerErrorCode(this, e);
            }
        }

        /// <summary>
        /// Текущие показания веса
        /// </summary>
        public int Weight
        {
            get 
            {
                try
                {
                    // определяем коммуникатор
                    using (ICommunicator communicator = GetCommunicator())
                    {
                        // устанавливаем соединение с устройством
                        communicator.Open();
                        OnOpen(communicator);

                        // определяем показания веса
                        int weight = GetWeight(communicator);

                        // формируем код ошибки
                        ErrorCode = new ServerErrorCode(this, GeneralError.Success);

                        // возвращаем показания веса
                        return weight;
                    }
                }
                catch (CommunicationException e)
                {
                    ErrorCode = new ServerErrorCode(this, e);
                    return -1;
                }
            }
        }

        #endregion

        /// <summary>
        /// Активация и деактивация устройства
        /// </summary>
        public override bool Active
        {
            get { return true; }
            set { }
        }
    }
}