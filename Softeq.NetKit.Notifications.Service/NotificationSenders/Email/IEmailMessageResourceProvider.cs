using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email
{
    public interface IEmailMessageResourceProvider
    {
        string GetBaseTemplate();
        string GetMainTemplate(string prefix, LanguageName language);
        string GetSubjectString(string name, LanguageName language);
    }
}