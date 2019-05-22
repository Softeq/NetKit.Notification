// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.SmsNotifications.Exception;
using Softeq.NetKit.Services.SmsNotifications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Softeq.NetKit.Services.SmsNotifications.SmsSender
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioSmsConfiguration _twilioSmsConfiguration;
        public TwilioSmsSender(TwilioSmsConfiguration twilioSmsConfiguration)
        {
            _twilioSmsConfiguration = twilioSmsConfiguration;
            InitTwilioClient();
        }

        private void InitTwilioClient()
        {
            if (string.IsNullOrEmpty(_twilioSmsConfiguration.AccountSid) || string.IsNullOrEmpty(_twilioSmsConfiguration.AuthToken) || string.IsNullOrEmpty(_twilioSmsConfiguration.FromNumber))
            {
                throw new SmsSenderException($"Error while sending sms. Config file does not contain one or more parameters for twilio client!");
            }
            
            TwilioClient.Init(_twilioSmsConfiguration.AccountSid, _twilioSmsConfiguration.AuthToken);
        }

        public async Task SendAsync(SmsDto sms)
        {
            try
            {
                var messageResponse = await MessageResource.CreateAsync(new CreateMessageOptions(sms.ToNumber)
                {
                    Body = sms.Text,
                    From = _twilioSmsConfiguration.FromNumber
                });

                if (messageResponse.Status == MessageResource.StatusEnum.Failed || messageResponse.Status == MessageResource.StatusEnum.Undelivered)
                {
                    var errors = new Dictionary<string, dynamic>
                    {
                        { "messageResponse", messageResponse }
                    };
                    throw new SmsSenderException($"Error while sending sms to {sms.ToNumber}", errors);
                }
            }
            catch (System.Exception e) when(!(e is SmsSenderException))
            {
                throw new SmsSenderException("Error while sending sms!", e);
            }
        }
    }
}
