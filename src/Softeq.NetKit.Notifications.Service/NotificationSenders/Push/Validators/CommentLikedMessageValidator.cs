// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Validators
{
    internal class CommentLikedMessageValidator : BasePushMessageValidator<CommentLikedPushMessage>
    {
        public CommentLikedMessageValidator()
        {
            RuleFor(x => x.UserIdWhoLikedComment).NotEmpty();
            RuleFor(x => x.UserNameWhoLikedComment).NotEmpty();
        }
    }
}
