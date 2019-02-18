// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Email
{
    public class EmailMessageValidatorTests
    {
        private static readonly MessageValidatorProvider Provider = new MessageValidatorProvider();
        private readonly IList<IEmailNotification> invalidMessages = new List<IEmailNotification>
        {
            new ForgotPasswordEmailMessage("","",new ForgotPasswordEmailModel()),
            new PackageArrivedEmailMessage("","", new PackageArrivedEmailModel())
        };

        private readonly IList<IEmailNotification> validMessages = new List<IEmailNotification>
        {
            new ForgotPasswordEmailMessage("alex@softeq","alex",new ForgotPasswordEmailModel
            {
                Name = "Alex",
                Link = "localhost"
            })
            {
                Subject = "PackageArrived",
                BaseHtmlTemplate = "template",
                HtmlTemplate = "template"
            },
            new PackageArrivedEmailMessage("alex@softeq","alex", new PackageArrivedEmailModel
            {
                OrderId=Guid.NewGuid().ToString(),
                TrackingNumber=Guid.NewGuid().ToString(),
                UserName="Alex"
            })
            {
                Subject = "PackageArrived",
                BaseHtmlTemplate = "template",
                HtmlTemplate = "template"
            }
        };

        [Fact]
        [Trait("Category", "Unit")]
        public void SuccessfulValidationTest()
        {
            foreach (var msg in validMessages)
            {
                var validator = Provider.GetValidator(msg);
                var result = validator.Validate(msg);
                Assert.True(result.IsValid);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void UnsuccessfulValidationTest()
        {
            foreach (var msg in invalidMessages)
            {
                var validator = Provider.GetValidator(msg);
                var result = validator.Validate(msg);
                Assert.False(result.IsValid);
                Assert.True(result.Errors.Any());
            }
        }
    }
}
