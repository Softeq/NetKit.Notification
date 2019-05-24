// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.SmsNotifications.Exception;
using Softeq.NetKit.Services.SmsNotifications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Softeq.NetKit.Services.SmsNotifications.SmsSender
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioSmsConfiguration _twilioSmsConfiguration;
        private readonly ITwilioRestClient _twilioRestClient;
        private const string ErrorWhileSendingSms = "Error while sending sms!";
        private const string ErrorWhileSendingSmsTo = "Error while sending sms to {0}";

        public TwilioSmsSender(TwilioSmsConfiguration twilioSmsConfiguration, ITwilioRestClient twilioRestClient)
        {
            _twilioSmsConfiguration = twilioSmsConfiguration;
            _twilioRestClient = twilioRestClient;
        }

        public async Task SendAsync(SmsDto sms)
        {
            try
            {
                var messageResponse = await MessageResource.CreateAsync(new CreateMessageOptions(sms.ToNumber)
                {
                    Body = sms.Text,
                    From = _twilioSmsConfiguration.FromNumber
                }, _twilioRestClient);

                if (messageResponse.Status == MessageResource.StatusEnum.Failed || messageResponse.Status == MessageResource.StatusEnum.Undelivered)
                {
                    var errors = new Dictionary<string, dynamic>
                    {
                        { "messageResponse", messageResponse }
                    };
                    throw new SmsSenderException(string.Format(ErrorWhileSendingSmsTo, sms.ToNumber), errors);
                }
            }
            catch (System.Exception e) when(!(e is SmsSenderException))
            {
                throw new SmsSenderException(ErrorWhileSendingSms, e);
            }
        }
    }
}
