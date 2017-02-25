using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using ERPService.SharedLibs.Helpers;

namespace StdGSMModemSMS
{
    [SMSClient(DeviceNames.standardGSMModem)]
    public class StandardGSMModemSMSClient : CustomSMSClient
    {
        private Boolean _active = false;

        protected override EncodedMessage[] OnEncode(String messageText, 
            PhoneNumber recipient)
        {
            ShortMessageEncoder encoder = new ShortMessageEncoder();
            encoder.MessageText = messageText;
            encoder.Recipient = recipient;
            if (ConnectivityParams.ContainsKey("SMS-сервер"))
            {
                encoder.SmsServer = new PhoneNumber(ConnectivityParams["SMS-сервер"]);
            }
            if (ConnectivityParams.ContainsKey("Время жизни сообщений"))
            {
                encoder.ValidityPeriod = Convert.ToInt32(
                    ConnectivityParams["Время жизни сообщений"]);
            }

            return encoder.Encode();
        }

        protected override void OnSend(EncodedMessage[] messages)
        {
            using (ShortMessageSender sender = new ShortMessageSender(
                ConnectivityParams["Порт"], Convert.ToInt32(ConnectivityParams["Скорость"])))
            {
                sender.Send(messages);
            }
        }

        public override Boolean Active
        {
            get { return _active; }
            set { _active = value; }
        }
    }
}
