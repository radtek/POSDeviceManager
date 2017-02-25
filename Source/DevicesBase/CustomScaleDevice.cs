using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase.Communicators;
using DevicesBase.Helpers;
using System.Xml;

namespace DevicesBase
{
    /// <summary>
    /// ��� ��������� ����������
    /// </summary>
    public enum AdvertisingType
    {
        /// <summary>
        /// ������������ ��������
        /// </summary>
        ShopName,

        /// <summary>
        /// ������� ������
        /// </summary>
        CreepingLine
    }

    /// <summary>
    /// ������� ����� ��� �����
    /// </summary>
    public abstract class CustomScaleDevice : CustomDevice, IScaleDevice
    {
        private String _connectionString;
        private ICommunicator _communicator;
        private Dictionary<String, Int32> _dataRecordsCount;

        /// <summary>
        /// ������� ����������-����
        /// </summary>
        protected CustomScaleDevice()
            : base()
        {
            _dataRecordsCount = new Dictionary<String, Int32>();
        }

        /// <summary>
        /// ���������� ��������� ������������� � ����������� �� ������ �����������
        /// </summary>
        private ICommunicator GetCommunicator()
        {
            if (_communicator == null)
            {
                // ���� ������������ ��� �� ������
                ConnStrHelper connStrHelper = new ConnStrHelper(_connectionString);
                
                // ���������� �������� �����
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
                            String.Format("�������� {0} �� ��������������.",
                            connStrHelper[1]));
                }

                return _communicator;
            }
            else
                // ������ �� ����� ��������� ��������� �������������
                return _communicator;
        }

        #region ��� ���������� � �������-��������

        /// <summary>
        /// ���. ������������� ������������ ����� ��������� ����������
        /// </summary>
        /// <param name="communicator">������������</param>
        protected virtual void OnOpen(ICommunicator communicator)
        {
        }

        /// <summary>
        /// ���������� ������� ��������� ����
        /// </summary>
        /// <param name="communicator">������������, ������������ � ����������</param>
        protected virtual Int32 GetWeight(ICommunicator communicator)
        {
            return 0;
        }

        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="articleName">������������ ������</param>
        /// <param name="articleCode">��� (�������) ������</param>
        /// <param name="PLU">����� ������ � ������ �����</param>
        /// <param name="price">���� �� ������� ����������� � ��������</param>
        /// <param name="units">���������� ����� � ������� �����������</param>
        /// <param name="box">��� ���� � �������</param>
        /// <param name="shelfLife">���� �������� � ����</param>
        /// <param name="message">����� ��������� (������-������)</param>
        protected virtual Boolean OnArticleUpload(String articleName, Int32 articleCode, Int32 PLU,
            Int32 price, Int32 units, Int32 box, Int32 shelfLife, Int32 message)
        {
            return true;
        }

        /// <summary>
        /// �������� ��������� (������-������)
        /// </summary>
        /// <param name="messageText">����� ���������</param>
        /// <param name="messageNumber">����� ��������� � ������ �����</param>
        protected virtual Boolean OnMessageUpload(String messageText, Int32 messageNumber)
        {
            return true;
        }

        /// <summary>
        /// �������� ��������� ����������
        /// </summary>
        /// <param name="advertisingText">����� ��������� ����������</param>
        /// <param name="advertisingType">��� ��������� ���������� <see cref="AdvertisingType"/></param>
        protected virtual Boolean OnAdvertisingUpload(String advertisingText,
            AdvertisingType advertisingType)
        {
            return true;
        }

        #endregion

        #region �������� ������

        /// <summary>
        /// ���������� �������� �������� (Int32)
        /// </summary>
        /// <param name="element">Xml-�������</param>
        /// <param name="attributeName">��� ��������</param>
        /// <param name="required">������������ ��� ���</param>
        /// <param name="defaultValue">�������� �� ���������</param>
        private Int32 GetInt32AttributeValue(XmlElement element, String attributeName,
            Boolean required, Int32 defaultValue)
        {
            if (!element.HasAttribute(attributeName))
            {
                if (required)
                    throw new Exception(String.Format("�� ����� ������������ ������� \"{0}\"", attributeName));
                else
                    return defaultValue;
            }

            return Convert.ToInt32(element.GetAttribute(attributeName));
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        /// <param name="articlesRoot">xml-������� "������"</param>
        /// <param name="startIndex">������ ������, � ������� �������� ��������</param>
        private Boolean UploadArticles(XmlElement articlesRoot, Int32 startIndex)
        {
            if (startIndex < 0)
                return true;

            Int32 itemsProceeded = 0;
            Boolean uploadResult = false;

            for (Int32 i = startIndex - 1; i < articlesRoot.ChildNodes.Count; i++)
            {
                // ��������� �����
                XmlElement article = (XmlElement)articlesRoot.ChildNodes[i];

                // �������� 
                uploadResult = OnArticleUpload(
                    article.InnerText,
                    GetInt32AttributeValue(article, "code", true, 0),
                    GetInt32AttributeValue(article, "plu", true, 0),
                    GetInt32AttributeValue(article, "price", false, 0),
                    GetInt32AttributeValue(article, "units", false, 1000),
                    GetInt32AttributeValue(article, "box", false, 0),
                    GetInt32AttributeValue(article, "shelfLife", false, 0),
                    GetInt32AttributeValue(article, "message", false, 0));
                // ����������� ���������� ����������� �������
                itemsProceeded++;
            }

            // ������������� �������� ����� ������������ �������
            _dataRecordsCount["articles"] += itemsProceeded;
            return uploadResult;
        }

        /// <summary>
        /// �������� ��������� 
        /// </summary>
        /// <param name="messagesRoot">xml-������� "���������"</param>
        /// <param name="startIndex">������ ������, � ������� �������� ��������</param>
        private Boolean UploadMessages(XmlElement messagesRoot, Int32 startIndex)
        {
            if (startIndex < 0)
                return true;

            Int32 itemsProceeded = 0;
            Boolean uploadResult = false;

            for (Int32 i = startIndex - 1; i < messagesRoot.ChildNodes.Count; i++)
            {
                // ��������� ���������
                XmlElement message = (XmlElement)messagesRoot.ChildNodes[i];

                // �������� 
                uploadResult = OnMessageUpload(
                    message.InnerText,
                    GetInt32AttributeValue(message, "number", false, 1));
                // ����������� ���������� ����������� �������
                itemsProceeded++;
            }

            // ������������� �������� ����� ������������ �������
            _dataRecordsCount["messages"] += itemsProceeded;
            return uploadResult;
        }

        /// <summary>
        /// �������� ��������� ����������
        /// </summary>
        /// <param name="advertisingsRoot"></param>
        /// <param name="startIndex">������ ������, � ������� �������� ��������</param>
        private Boolean UploadAdvertisings(XmlElement advertisingsRoot, Int32 startIndex)
        {
            if (startIndex < 0)
                return true;

            Int32 itemsProceeded = 0;
            Boolean uploadResult = false;

            for (Int32 i = startIndex - 1; i < advertisingsRoot.ChildNodes.Count; i++)
            {
                // ��������� ������ ��������� ����������
                XmlElement advertising = (XmlElement)advertisingsRoot.ChildNodes[i];

                // �������� 
                uploadResult = OnAdvertisingUpload(
                    advertising.InnerText,
                    advertising.GetAttribute("type") == "shopName" ? AdvertisingType.ShopName :
                        AdvertisingType.CreepingLine);
                // ����������� ���������� ����������� �������
                itemsProceeded++;
            }

            // ������������� �������� ����� ������������ �������
            _dataRecordsCount["messages"] += itemsProceeded;
            return uploadResult;
        }

        #endregion

        #region ���������� IScaleDevice

        /// <summary>
        /// ������ ����������� � �����
        /// </summary>
        public String ConnectionString
        {
            get { return _connectionString; }
            set 
            { 
                // ���������� ������ ����������� - ����� ����������� ������������
                _communicator = null;
                _connectionString = value; 
            }
        }

        /// <summary>
        /// �������� ������ � ����
        /// </summary>
        /// <param name="xmlData">������</param>
        public void Upload(string xmlData)
        {
            try
            {
                // ���������� ������������
                using (ICommunicator communicator = GetCommunicator())
                {
                    // ������������� ���������� � �����������
                    communicator.Open();
                    OnOpen(communicator);

                    // ������� ������� ��������� � ��������
                    _dataRecordsCount.Clear();
                    _dataRecordsCount.Add("articles", 0);
                    _dataRecordsCount.Add("messages", 0);
                    _dataRecordsCount.Add("advertisings", 0);

                    // ������� xml-��������
                    XmlDocument uploadData = new XmlDocument();
                    uploadData.LoadXml(xmlData);
                    if (String.Compare(uploadData.DocumentElement.Name, "scale", true) != 0)
                        throw new CommunicationException("������ �� ������������� ��� �������� � ����");

                    // ��� ���� ������� ������
                    foreach (XmlElement uploadTag in uploadData.DocumentElement.ChildNodes)
                    {
                        // ���������� ����� ��������, � �������� �������� �������� ������
                        Int32 startIndex = GetInt32AttributeValue(uploadTag, "startIndex", false, 1);

                        // ����� ������ ������
                        if (String.Compare(uploadTag.Name, "articles") == 0)
                        {
                            // ������
                            _dataRecordsCount["articles"] = startIndex;
                            if (!UploadArticles(uploadTag, startIndex))
                                break;
                            continue;
                        }

                        if (String.Compare(uploadTag.Name, "messages") == 0)
                        {
                            // ���������
                            _dataRecordsCount["messages"] = startIndex;
                            if (!UploadMessages(uploadTag, startIndex))
                                break;
                            continue;
                        }

                        if (String.Compare(uploadTag.Name, "advertisings") == 0)
                        {
                            // �������
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
        /// ������� ��������� ����
        /// </summary>
        public Int32 Weight
        {
            get 
            {
                try
                {
                    // ���������� ������������
                    using (ICommunicator communicator = GetCommunicator())
                    {
                        // ������������� ���������� � �����������
                        communicator.Open();
                        OnOpen(communicator);

                        // ���������� ��������� ����
                        Int32 weight = GetWeight(communicator);

                        // ��������� ��� ������
                        ErrorCode = new ServerErrorCode(this, GeneralError.Success);

                        // ���������� ��������� ����
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
        /// ��������� � ����������� ����������
        /// </summary>
        public override Boolean Active
        {
            get { return true; }
            set { }
        }
    }
}