// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Validators
{
    internal class PackageArrivedMessageValidator : BaseEmailValidator<PackageArrivedEmailMessage>
    {
        public PackageArrivedMessageValidator()
        {
            RuleFor(x => x.TemplateModel.OrderId).NotEmpty().OverridePropertyName(nameof(PackageArrivedEmailModel.OrderId));
            RuleFor(x => x.TemplateModel.TrackingNumber).NotEmpty().OverridePropertyName(nameof(PackageArrivedEmailModel.TrackingNumber));
            RuleFor(x => x.TemplateModel.UserName).NotEmpty().OverridePropertyName(nameof(PackageArrivedEmailModel.UserName));
        }
    }
}
