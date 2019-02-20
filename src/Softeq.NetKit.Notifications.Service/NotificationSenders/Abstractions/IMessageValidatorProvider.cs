// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions
{
    internal interface IMessageValidatorProvider
    {
        IValidator GetValidator<TModel>(TModel model);
    }
}