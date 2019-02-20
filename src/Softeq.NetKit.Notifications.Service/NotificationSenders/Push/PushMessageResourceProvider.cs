// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Localization;
using System.IO;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    internal class PushMessageResourceProvider : FileResourceProvider, IPushMessageResourceProvider
    {
        private static readonly string ResourceRootPath = Path.Combine(BasePath, @"Push\Resources");

        public string GetBody(string name, LanguageName language)
        {
            return GetString($"{name}Body", language);
        }

        public string GetTitle(string name, LanguageName language)
        {
            return GetString($"{name}Title", language);
        }

        protected override string ResourceStringPath => ResourceRootPath;
    }
}
