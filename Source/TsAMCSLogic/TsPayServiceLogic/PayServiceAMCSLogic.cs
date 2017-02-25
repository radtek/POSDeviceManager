using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using TsManager;
using DevicesCommon;

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
        private void GetRequest(String idData, Stream requestStream)
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
        private Int32 GetBalance(Stream responseStream, String idData)
        {
            // ������� ��������
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(responseStream);

            // ��������� ��������� ��������
            // ��� ������
            String attribValue = xmlDoc.DocumentElement.GetAttribute("errorCode");
            String errorMessage = xmlDoc.DocumentElement["errorMessage"].InnerText;
            if (String.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(String.Format(
                    "������ �� ��������. ������: \"{0}\" [{1}]", attribValue, errorMessage));

            // ��� �������
            attribValue = xmlDoc.DocumentElement.GetAttribute("statusCode");
            errorMessage = xmlDoc.DocumentElement["statusMessage"].InnerText;
            if (String.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(String.Format(
                    "������ ��������. ������: \"{0}\" [{1}]", attribValue, errorMessage));

            // ���� ������� ��� �����
            if (String.Compare(xmlDoc.DocumentElement["forbidden"].InnerText, "0") != 0)
                throw new InvalidOperationException(String.Format(
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

        public Boolean IsAccessGranted(TurnstileDirection direction, String idData,
            out String reason)
        {
            // ������� ������
            reason = "������ ��������";

            // ��������� HTTP-������ � ���
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(
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
                        Int32 balance = GetBalance(responseStream, idData);

                        // ���������� �� ����� ������ � ��������������� ��������
                        Boolean accessGranted = (direction == TurnstileDirection.Entry) ||
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

        public void OnAccessOccured(TurnstileDirection direction, String idData)
        {
        }

        #endregion
    }
}
