// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Messages;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Validators
{
    public abstract class BaseSmsValidator<TModel> : AbstractValidator<TModel> where TModel : SmsMessage
    {
        private const string MaximumSmsLengthExceptionMessage = "Maximum length for sms text is 1600 characters!";
        private const string MaximumPhoneNumberExceptionMessage = "Maximum length for phone number is 14 characters!";

        protected BaseSmsValidator()
        {
            RuleFor(message => message.Text).NotEmpty();
            RuleForEach(message => message.RecipientPhoneNumbers).NotEmpty();
            RuleFor(message => message.Text).MaximumLength(1600).WithMessage(MaximumSmsLengthExceptionMessage);
            RuleForEach(message => message.RecipientPhoneNumbers).MaximumLength(14).WithMessage(MaximumPhoneNumberExceptionMessage);
        }
    }
}
