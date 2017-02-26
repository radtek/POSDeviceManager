using System;
using System.IO;
using System.Net;
using System.Xml;
using DevicesCommon;
using TsManager;

namespace TsPayServiceLogic
{
    [AMCSLogicAttrubute("������-�: ������� � ������ - ��� ����� �� ����")]
    public class PayServiceAMCSLogic : IAMCSLogic
    {
        private PayServiceAMCSLogicSettings _settings;

        public PayServiceAMCSLogic()
        {
            _settings = new PayServiceAMCSLogicSettings();
        }

        #region �������� ������

        /// <summary>
        /// ��������� ������ � ���
        /// </summary>
        private void GetRequest(string idData, Stream requestStream)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // ��������� ���������
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "windows-1251", null);
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            // �������� ������� ���������
            XmlElement rootNode = xmlDoc.CreateElement("request");
            xmlDoc.AppendChild(rootNode);

            // �������� �������
            rootNode.SetAttribute("type", "info");
            rootNode.SetAttribute("account", idData);

            // ����� ���� ������� � �����
            using (StreamWriter sw = new StreamWriter(requestStream))
            {
                sw.Write(xmlDoc.OuterXml);
            }
        }

        /// <summary>
        /// ���������� ������ ���������� �����
        /// </summary>
        /// <param name="responseStream">����� ������ �� ������</param>
        /// <param name="idData">����������������� ������</param>
        /// <returns>������ ���������� �����</returns>
        private int GetBalance(Stream responseStream, string idData)
        {
            // ������� ��������
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(responseStream);

            // ��������� ��������� ��������
            // ��� ������
            string attribValue = xmlDoc.DocumentElement.GetAttribute("errorCode");
            string errorMessage = xmlDoc.DocumentElement["errorMessage"].InnerText;
            if (string.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(string.Format(
                    "������ �� ��������. ������: \"{0}\" [{1}]", attribValue, errorMessage));

            // ��� �������
            attribValue = xmlDoc.DocumentElement.GetAttribute("statusCode");
            errorMessage = xmlDoc.DocumentElement["statusMessage"].InnerText;
            if (string.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(string.Format(
                    "������ ��������. ������: \"{0}\" [{1}]", attribValue, errorMessage));

            // ���� ������� ��� �����
            if (string.Compare(xmlDoc.DocumentElement["forbidden"].InnerText, "0") != 0)
                throw new InvalidOperationException(string.Format(
                    "����� [{0}] ��������� � �������������", idData));

            // ������ �����
            return Convert.ToInt32(xmlDoc.DocumentElement["balance"].InnerText);
        }

        #endregion

        #region ���������� IAMCSLogic

        public Object Settings
        {
            get { return _settings; }
            set { _settings = (PayServiceAMCSLogicSettings)value; }
        }

        public bool IsAccessGranted(TurnstileDirection direction, string idData,
            out string reason)
        {
            // ������� ������
            reason = "������ ��������";

            // ��������� HTTP-������ � ���
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(
                "http://{0}:{1}/", _settings.HostOrIp, _settings.Port));
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                GetRequest(idData, requestStream);
            }

            try
            {
                // ����������� ���������� �� �����
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // ������ �����
                        int balance = GetBalance(responseStream, idData);

                        // ���������� �� ����� ������ � ��������������� ��������
                        bool accessGranted = (direction == TurnstileDirection.Entry) ||
                            (direction == TurnstileDirection.Exit && balance >= 0);
                        if (!accessGranted)
                            reason = "������������� ������ �����";

                        return accessGranted;
                    }
                }
            }
            catch (WebException e)
            {
                reason = e.Message;
                return false;
            }
            catch (InvalidOperationException e)
            {
                reason = e.Message;
                return false;
            }
        }

        public void OnAccessOccured(TurnstileDirection direction, string idData)
        {
        }

        #endregion
    }
}
