// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Services.EmailNotifications.EmailSender;
using Softeq.NetKit.Services.EmailNotifications.Models;
using Softeq.NetKit.Services.EmailNotifications.Tests.TestData;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Services.EmailNotifications.Tests
{
    public class EmailNotificationServiceTest : IClassFixture<ServiceTestFixture>
    {
        private readonly ServiceTestFixture _fixture;

        public EmailNotificationServiceTest(ServiceTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenMessageIsMissingArgumentNullExceptionIsThrown()
        {
            var source = new Mock<IEmailSender>();

            var emailService = new EmailNotificationService(source.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => emailService.SendAsync(null));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenMessageIsValidThenClientIsCalled()
        {
            var source = new Mock<IEmailSender>();

            SendEmailDto saveDto = null;

            source.Setup(c => c.SendAsync(It.IsAny<SendEmailDto>()))
                .Callback<SendEmailDto>((dto) => saveDto = dto)
                .Returns(Task.CompletedTask);

            var emailService = new EmailNotificationService(source.Object);
            var message = new TestEmailNotification(
                "alexander.zyl@softeq.com",
                "Softeq Unit Test",
                new TestEmailModel("google.com", "Softeq Unit Test"));

            await emailService.SendAsync(message);

            source.Verify(sender => sender.SendAsync(It.IsAny<SendEmailDto>()), Times.Once);
            Assert.NotNull(saveDto);
            Assert.Equal(message.Text, saveDto.Text);
            Assert.Equal(message.Recipients, saveDto.Recipients);
            Assert.Equal(message.Subject, saveDto.Subject);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenMessageIsTemplatedThenSubjectIsFormatted()
        {
            var source = new Mock<IEmailSender>();

            SendEmailDto saveDto = null;

            source.Setup(c => c.SendAsync(It.IsAny<SendEmailDto>()))
                .Callback<SendEmailDto>((dto) => saveDto = dto)
                .Returns(Task.CompletedTask);

            var emailService = new EmailNotificationService(source.Object);
            var message = new TemplatedTestEmailNotification(
                "alexander.zyl@softeq.com",
                "Softeq Unit Test",
                new TestEmailModel("google.com", "Softeq Unit Test"));

            await emailService.SendAsync(message);

            var expectedSubject = string.Format(message.Subject, message.TemplateModel.Name);

            source.Verify(sender => sender.SendAsync(It.IsAny<SendEmailDto>()), Times.Once);
            Assert.NotNull(saveDto);
            Assert.Equal(message.Text, saveDto.Text);
            Assert.Equal(message.Recipients, saveDto.Recipients);
            Assert.Equal(expectedSubject, saveDto.Subject);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SendForgotPasswordEmailSuccessfully()
        {
            IEmailSender sender = new SendGridEmailSender(_fixture.Configuration);
            var emailService = new EmailNotificationService(sender);
            await emailService.SendAsync(new TestEmailNotification(
                "alexander.zyl@softeq.com",
                "Softeq Unit Test",
                new TestEmailModel("google.com", "Softeq Unit Test")));
        }
    }
}
