// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Validators
{
    internal abstract class BasePushMessageValidator<TModel> : AbstractValidator<TModel> where TModel : PushNotificationMessage
    {
        protected BasePushMessageValidator()
        {
        }
    }
}
