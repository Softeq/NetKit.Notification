// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Validators
{
    internal class ArticleCreatedMessageValidator : BasePushMessageValidator<ArticleCreatedPushMessage>
    {
        public ArticleCreatedMessageValidator()
        {
            RuleFor(x => x.ArticleId).NotEqual(Guid.Empty);
        }
    }
}
