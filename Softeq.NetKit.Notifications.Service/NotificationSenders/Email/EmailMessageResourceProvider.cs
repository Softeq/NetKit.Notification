// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Service.Utility;
using System.Collections.Concurrent;
using System.IO;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email
{
    internal class EmailMessageResourceProvider : FileResourceProvider, IEmailMessageResourceProvider
    {
        static EmailMessageResourceProvider()
        {
            BaseTemplate = File.ReadAllText(Path.Combine(TemplateRootPath, BaseTemplateName));
        }

        private static readonly string BaseTemplate;
        private static readonly ConcurrentDictionary<string, string> TemplateCache = new ConcurrentDictionary<string, string>();
        private static readonly string ResourceRootPath = Path.Combine(BasePath, @"Email\Resources");
        private static readonly string TemplateRootPath = Path.Combine(ResourceRootPath, @"Templates");
        private const string BaseTemplateName = "BaseEmail.html";

        public string GetBaseTemplate() => BaseTemplate;

        public string GetMainTemplate(string prefix, LanguageName language)
        {
            var langTemplateName = $"{prefix}.{language.GetLanguageCode()}.html";
            return TemplateCache.GetOrAdd(langTemplateName, s =>
            {
                var messageTemplate = File.ReadAllText(Path.Combine(TemplateRootPath, langTemplateName));
                return messageTemplate;
            });
        }

        public string GetSubjectString(string name, LanguageName language)
        {
            return GetString($"{name}Subject", language);
        }

        protected override string ResourceStringPath => ResourceRootPath;
    }
}
