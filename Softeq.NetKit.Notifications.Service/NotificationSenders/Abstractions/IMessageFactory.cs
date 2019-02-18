// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions
{
    internal interface IMessageFactory<out TMessage>
    {
        TMessage Create(NotificationMessage message, UserProfileSettings settings);
    }
}
