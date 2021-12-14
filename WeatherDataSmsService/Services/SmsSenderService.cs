using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace WeatherDataSmsService
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        private readonly string _toPhoneNumber;

        public SmsSenderService(string accountSid, string authToken, string fromPhoneNumber, string toPhoneNumber)
        {
            this._accountSid = accountSid;
            this._authToken = authToken;
            TwilioClient.Init(_accountSid, _authToken);

            this._fromPhoneNumber = fromPhoneNumber;
            this._toPhoneNumber = toPhoneNumber;
        }

        public void SendSMS(string content)
        {
            var message = MessageResource.Create(
                body: content,
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(_toPhoneNumber)
            );

           Console.WriteLine($"Message Sent: {message.Sid}");
        }
    }
}
