// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Validators
{
    internal class ForgotPasswordMessageValidator : BaseEmailValidator<ForgotPasswordEmailMessage>
    {
        public ForgotPasswordMessageValidator()
        {
            RuleFor(x => x.TemplateModel.Name).NotEmpty().OverridePropertyName(nameof(ForgotPasswordEmailModel.Name));
            RuleFor(x => x.TemplateModel.Link).NotEmpty().OverridePropertyName(nameof(ForgotPasswordEmailModel.Link));
        }
    }
}
