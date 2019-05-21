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
    class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioSmsConfiguration _twilioSmsConfiguration;
        public TwilioSmsSender(TwilioSmsConfiguration twilioSmsConfiguration)
        {
            _twilioSmsConfiguration = twilioSmsConfiguration;
           TwilioClient.Init(twilioSmsConfiguration.AccountSid, twilioSmsConfiguration.AuthToken);
        }

        public async Task SendAsync(SendSmsDto sms)
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
