// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Services.SmsNotifications.Abstract;
using Softeq.NetKit.Services.SmsNotifications.SmsSender;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Services.SmsNotifications.Tests
{
    public class SmsNotificationServiceTest : IClassFixture<SmsNotificationServiceTestConfiguration>
    {
        private const string SmsText = "smsText";
        private readonly SmsNotificationServiceTestConfiguration _smsConfiguration;

        public SmsNotificationServiceTest(SmsNotificationServiceTestConfiguration smsConfiguration)
        {
            _smsConfiguration = smsConfiguration;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SendSmsWithoutParametersShouldThrowException()
        {
            ISmsNotification emptySmsNotification = null;
            var source = new Mock<ISmsSender>();
            TwilioSmsConfiguration emptyConfig = new TwilioSmsConfiguration();
            var mockSmsService = new SmsNotificationService(source.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => mockSmsService.SendAsync(emptySmsNotification));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SendSmsNotification()
        {
            var smsSender = new TwilioSmsSender(_smsConfiguration.Configuration, _smsConfiguration.TwilioRestClient);
            var smsService = new SmsNotificationService(smsSender);
            var recipients = new List<string>() {_smsConfiguration.RecipientPhoneNumber};
            var sms = new BaseSmsNotification() {Text = SmsText, RecipientPhoneNumbers = recipients};

            await smsService.SendAsync(sms);
        }
    }
}
