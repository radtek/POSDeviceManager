using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevicesCommon;
using TsManager;

namespace TsBiometricsLogic
{
    [AMCSLogicAttrubute("Форинт CMS: Биометрическая авторизация")]
    public class CMSBiometricsLogic : IAMCSLogic
    {
        private static string CLIENT_REQUEST = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><request type=\"client_info\" account=\"\"><biometrics>{0}</biometrics></request>";
        private static string INFO_REQUEST = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><request type=\"info\" account=\"{0}\"></request>";
        private static string REGISTER_REQUEST = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><request type=\"registration\" account=\"{0}\"><pointId>{1}</pointId><posId>{2}</posId></request>";

        private CMSBiometricsLogicSettings _settings = new CMSBiometricsLogicSettings();

        #region IAMCSLogic Members

        public bool IsAccessGranted(TurnstileDirection direction, string idData, out string reason)
        {
            // причина отказа
            reason = "Доступ разрешен";

            try
            {
                // запрос client_info, получаем номер карты
                string cardNo = GetCardNo(idData);

                // запрос info, проверяем состояние счета
                if (!CheckBalance(cardNo, _settings.MinBalance))
                    throw new InvalidOperationException("Недостаточно средств на счете клиента");

                // запрос registration, регистрируем посещение
                if (_settings.RegisterVisit)
                    RegisterVisit(cardNo, _settings.PointId, _settings.TerminalNo);

                return true;
            }
            catch (WebException E)
            {
                reason = E.Message;
                return false;
            }
            catch (InvalidOperationException E)
            {
                reason = E.Message;
                return false;
            }
        }

        private bool CheckBalance(string cardNo, decimal minBalance)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format(
                "http://{0}:{1}/", _settings.HostOrIp, _settings.Port));
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                var requestBytes = Encoding.GetEncoding(1251).GetBytes(string.Format(INFO_REQUEST, cardNo));
                requestStream.Write(requestBytes, 0, requestBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (var reader = XmlTextReader.Create(response.GetResponseStream()))
            {
                var responseXmlDoc = XDocument.Load(reader);
                var responseXml = responseXmlDoc.Element("response");
                int errorCode = Convert.ToInt32(responseXml.Attribute("errorCode").Value);
                if (errorCode != 0)
                    throw new InvalidOperationException(responseXml.Element("errorMessage").Value);
                int statusCode = Convert.ToInt32(responseXml.Attribute("statusCode").Value);
                if (statusCode != 0)
                    throw new InvalidOperationException(responseXml.Element("statusMessage").Value);

                // состояние счета и карты
                if(responseXml.Element("forbidden") != null && responseXml.Element("forbidden").Value == "1")
                    throw new InvalidOperationException("Счет клиента заблокирован");

                // баланс
                if (responseXml.Element("balance") != null)
                {
                    int balance = Convert.ToInt32(responseXml.Element("balance").Value);
                    return (decimal)balance / 100m >= minBalance;
                }

                return false;
            }
        }

        private string GetCardNo(string idData)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format(
            "http://{0}:{1}/", _settings.HostOrIp, _settings.Port));
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                var requestBytes = Encoding.GetEncoding(1251).GetBytes(string.Format(CLIENT_REQUEST, idData));
                requestStream.Write(requestBytes, 0, requestBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (var reader = XmlTextReader.Create(response.GetResponseStream()))
            {
                var responseXmlDoc = XDocument.Load(reader);
                var responseXml = responseXmlDoc.Element("response");

                int errorCode = Convert.ToInt32(responseXml.Attribute("errorCode").Value);
                if (errorCode != 0)
                    throw new InvalidOperationException(responseXml.Element("errorMessage").Value);
                int statusCode = Convert.ToInt32(responseXml.Attribute("statusCode").Value);
                if (statusCode != 0)
                    throw new InvalidOperationException(responseXml.Element("statusMessage").Value);

                foreach (var client in responseXml.Elements("client"))
                    foreach (var account in client.Elements("account"))
                        foreach (var card in account.Elements("card"))
                            return card.Value;
            }

            throw new InvalidOperationException("Клиент не найден");
        }

        private void RegisterVisit(string cardNo, string point, int terminal)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format(
            "http://{0}:{1}/", _settings.HostOrIp, _settings.Port));
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                var requestBytes = Encoding.GetEncoding(1251).GetBytes(string.Format(REGISTER_REQUEST, cardNo, point, terminal));
                requestStream.Write(requestBytes, 0, requestBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (var reader = XmlTextReader.Create(response.GetResponseStream()))
            {
                var responseXmlDoc = XDocument.Load(reader);
                var responseXml = responseXmlDoc.Element("response");

                int errorCode = Convert.ToInt32(responseXml.Attribute("errorCode").Value);
                if (errorCode != 0)
                    throw new InvalidOperationException(responseXml.Element("errorMessage").Value);
                int statusCode = Convert.ToInt32(responseXml.Attribute("statusCode").Value);
                if (statusCode != 0)
                    throw new InvalidOperationException(responseXml.Element("statusMessage").Value);
            }
        }

        public void OnAccessOccured(TurnstileDirection direction, string idData)
        {
        }

        public object Settings
        {
            get { return _settings; }
            set { _settings = (CMSBiometricsLogicSettings)value; }
        }

        #endregion
    }
}
