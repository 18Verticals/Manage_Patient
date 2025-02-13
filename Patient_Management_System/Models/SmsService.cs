using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Patient_Management_System.Services
{
    public class SmsService
    {
        private const string AccountSid = "Your_Twilio_Account_SID";
        private const string AuthToken = "Your_Twilio_Auth_Token";
        private const string TwilioPhoneNumber = "+Your_Twilio_Number";

        public void SendSms(string phoneNumber, string message)
        {
            TwilioClient.Init(AccountSid, AuthToken);

            var messageResult = MessageResource.Create(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber(TwilioPhoneNumber),
                body: message
            );

            Console.WriteLine($"SMS Sent: {messageResult.Sid}");
        }
    }
}
