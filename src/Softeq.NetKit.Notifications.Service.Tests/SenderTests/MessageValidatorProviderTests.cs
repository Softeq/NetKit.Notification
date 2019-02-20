// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests
{
    public class MessageValidatorProviderTests
    {
        private static readonly MessageValidatorProvider Provider = new MessageValidatorProvider();

        [Fact]
        [Trait("Category", "Unit")]
        public void WhenMessageIsSupportedThenValidatorReturnedTest()
        {
            var validator = Provider.GetValidator(new ArticleCreatedPushMessage());
            Assert.NotNull(validator);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WhenMessageIsNotSupportedThenUnsupportedExceptionIsThrown()
        {
            Assert.Throws<InvalidOperationException>(() => Provider.GetValidator(new SomeClass()));
        }

        private class SomeClass
        {

        }
    }
}
