// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Messages;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Validators
{
    public abstract class BaseSmsValidator<TModel> : AbstractValidator<TModel> where TModel : SmsMessage
    {
        private const string MaximumSmsLengthExceptionMessage = "Maximum length for sms text is 1600 characters!";
        private const string MaximumPhoneNumberExceptionMessage = "Maximum length for phone number is 12 characters!";

        protected BaseSmsValidator()
        {
            RuleFor(message => message.Text).NotEmpty();
            RuleFor(message => message.RecipientPhoneNumber).NotEmpty();
            RuleFor(message => message.Text).MaximumLength(1600).WithMessage(MaximumSmsLengthExceptionMessage);
            RuleFor(message => message.RecipientPhoneNumber).MaximumLength(12).WithMessage(MaximumPhoneNumberExceptionMessage);
        }
    }
}
