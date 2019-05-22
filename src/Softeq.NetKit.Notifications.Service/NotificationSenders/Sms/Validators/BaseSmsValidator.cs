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
            RuleFor(message => message.RecipientPhoneNumber).NotEmpty();
            RuleFor(message => message.Text).MaximumLength(1600).WithMessage("Maximum length for sms text is 1600 characters!");
            RuleFor(message => message.RecipientPhoneNumber).MaximumLength(12).WithMessage("Maximum length for phone number is 12 characters!");
        }
    }
}
