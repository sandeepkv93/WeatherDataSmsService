using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace WeatherDataSmsService
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public SmsSenderService(string accountSid, string authToken)
        {
            this._accountSid = accountSid;
            this._authToken = authToken;
            TwilioClient.Init(_accountSid, _authToken);
        }

        public void SendSMS(string content)
        {
            var message = MessageResource.Create(
                body: content,
                from: new Twilio.Types.PhoneNumber("+15305615949"),
                to: new Twilio.Types.PhoneNumber("+16319979230")
            );

           Console.WriteLine($"Message Sent: {message.Sid}");
        }
    }
}
