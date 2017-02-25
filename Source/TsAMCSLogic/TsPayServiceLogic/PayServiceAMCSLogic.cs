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
    [AMCSLogicAttrubute("Форинт-С: Платежи и скидки - без платы за вход")]
    public class PayServiceAMCSLogic : IAMCSLogic
    {
        private PayServiceAMCSLogicSettings _settings;

        public PayServiceAMCSLogic()
        {
            _settings = new PayServiceAMCSLogicSettings();
        }

        #region Закрытые методы

        /// <summary>
        /// Формирует запрос к ПДС
        /// </summary>
        private void GetRequest(String idData, Stream requestStream)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // заголовок документа
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "windows-1251", null);
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            // корневой элемент документа
            XmlElement rootNode = xmlDoc.CreateElement("request");
            xmlDoc.AppendChild(rootNode);

            // атрибуты запроса
            rootNode.SetAttribute("type", "info");
            rootNode.SetAttribute("account", idData);

            // пишем текс запроса в поток
            using (StreamWriter sw = new StreamWriter(requestStream))
            {
                sw.Write(xmlDoc.OuterXml);
            }
        }

        /// <summary>
        /// Возвращает баланс карточного счета
        /// </summary>
        /// <param name="responseStream">Поток ответа на запрос</param>
        /// <param name="idData">Идентификационные данные</param>
        /// <returns>Баланс карточного счета</returns>
        private Int32 GetBalance(Stream responseStream, String idData)
        {
            // создаем документ
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(responseStream);

            // проверяем статусные атрибуты
            // код ошибки
            String attribValue = xmlDoc.DocumentElement.GetAttribute("errorCode");
            String errorMessage = xmlDoc.DocumentElement["errorMessage"].InnerText;
            if (String.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(String.Format(
                    "Запрос не выполнен. Ошибка: \"{0}\" [{1}]", attribValue, errorMessage));

            // код статуса
            attribValue = xmlDoc.DocumentElement.GetAttribute("statusCode");
            errorMessage = xmlDoc.DocumentElement["statusMessage"].InnerText;
            if (String.Compare(attribValue, "0") != 0)
                throw new InvalidOperationException(String.Format(
                    "Запрос выполнен. Ошибка: \"{0}\" [{1}]", attribValue, errorMessage));

            // флаг запрета для карты
            if (String.Compare(xmlDoc.DocumentElement["forbidden"].InnerText, "0") != 0)
                throw new InvalidOperationException(String.Format(
                    "Карта [{0}] запрещена к использованию", idData));

            // баланс карты
            return Convert.ToInt32(xmlDoc.DocumentElement["balance"].InnerText);
        }

        #endregion

        #region Реализация IAMCSLogic

        public Object Settings
        {
            get { return _settings; }
            set { _settings = (PayServiceAMCSLogicSettings)value; }
        }

        public Boolean IsAccessGranted(TurnstileDirection direction, String idData,
            out String reason)
        {
            // причина отказа
            reason = "Доступ разрешен";

            // формируем HTTP-запрос к ПДС
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(
                "http://{0}:{1}/", _settings.HostOrIp, _settings.Port));
            request.Method = "POST";
            using (Stream requestStream = request.GetRequestStream())
            {
                GetRequest(idData, requestStream);
            }

            try
            {
                // запрашиваем информацию по карте
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // читаем ответ
                        Int32 balance = GetBalance(responseStream, idData);

                        // пропускаем на выход только с неотрицательным балансом
                        Boolean accessGranted = (direction == TurnstileDirection.Entry) ||
                            (direction == TurnstileDirection.Exit && balance >= 0);
                        if (!accessGranted)
                            reason = "Отрицательный баланс счета";

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
