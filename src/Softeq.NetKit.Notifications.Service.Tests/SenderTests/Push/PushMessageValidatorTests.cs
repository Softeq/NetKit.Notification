// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Services.PushNotifications.Models;
using System.Collections.Generic;
using System.Linq;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Push
{
    public class PushMessageValidatorTests
    {
        private static readonly MessageValidatorProvider Provider = new MessageValidatorProvider();
        private readonly IList<PushNotificationMessage> validMessages = new List<PushNotificationMessage>
        {
            new ArticleCreatedPushMessage
            {
                ArticleId = Guid.NewGuid()
            },
            new CommentLikedPushMessage
            {
                UserNameWhoLikedComment = "Alex",
                UserIdWhoLikedComment = Guid.NewGuid().ToString()
            }
        };

        private readonly IList<PushNotificationMessage> invalidMessages = new List<PushNotificationMessage>
        {
            new ArticleCreatedPushMessage
            {
                ArticleId = Guid.Empty
            },
            new CommentLikedPushMessage
            {
                UserNameWhoLikedComment = null,
                UserIdWhoLikedComment = null
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
