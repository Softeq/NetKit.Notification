// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Messages;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Validators
{
    public abstract class BaseSmsValidator<TModel> : AbstractValidator<TModel> where TModel : SmsMessage
    {
        protected BaseSmsValidator()
        {
            RuleFor(message => message.Text).NotEmpty();
            RuleFor(message => message.Recipient).NotEmpty();
        }
    }
}
