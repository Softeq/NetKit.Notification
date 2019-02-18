// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Services.EmailNotifications.Abstract;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Validators
{
    internal abstract class BaseEmailValidator<TMessage> : AbstractValidator<TMessage> where TMessage : IEmailNotification
    {
        protected BaseEmailValidator()
        {
            RuleFor(message => message.Subject).NotEmpty();
            RuleFor(message => message.Recipients).NotEmpty().WithMessage("Email recipient list is empty");
            RuleFor(message => message.HtmlTemplate).NotEmpty();
            RuleFor(message => message.BaseHtmlTemplate).NotEmpty();
        }
    }
}
