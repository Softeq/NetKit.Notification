// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Http;

namespace Softeq.NetKit.Services.SmsNotifications.SmsSender
{
    public class TwilioHttpClient : ITwilioRestClient
    {
        private readonly ITwilioRestClient _innerClient;

        public TwilioHttpClient(System.Net.Http.HttpClient httpClient, TwilioSmsConfiguration twilioSmsConfiguration)
        {
            _innerClient = new TwilioRestClient(
                twilioSmsConfiguration.AccountSid,
                twilioSmsConfiguration.AuthToken,
                httpClient: new SystemNetHttpClient(httpClient));
        }

        public Response Request(Request request) => _innerClient.Request(request);
        public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
        public string AccountSid => _innerClient.AccountSid;
        public string Region => _innerClient.Region;
        public Twilio.Http.HttpClient HttpClient => _innerClient.HttpClient;
    }
}
