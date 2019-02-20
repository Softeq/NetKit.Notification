using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    public interface IPushMessageResourceProvider
    {
        string GetBody(string name, LanguageName language);
        string GetTitle(string name, LanguageName language);
    }
}