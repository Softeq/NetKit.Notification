// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Service.Utility;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders
{
    internal abstract class FileResourceProvider
    { 
        private Dictionary<LanguageName, Dictionary<string, string>> _stringCache;
        protected static string BasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NotificationSenders");

        protected FileResourceProvider()
        {
            InitializeStringResources();
        }

        protected string GetString(string name, LanguageName lang)
        {
            return _stringCache[lang].TryGetValue(name, out var locString) 
                ? locString
                : string.Empty;
        }

        protected abstract string ResourceStringPath { get; }

        private void InitializeStringResources()
        {
            if (_stringCache != null)
            {
                return;
            }

            var cache = new Dictionary<LanguageName, Dictionary<string, string>>();

            foreach (LanguageName enumVal in Enum.GetValues(typeof(LanguageName)))
            {
                var code = enumVal.GetLanguageCode();
                var stringsPath = Path.Combine(ResourceStringPath, $"LocalizationStrings.{code}.json");
                var fileContent = File.ReadAllText(stringsPath);

                var stringMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
                cache.Add(enumVal, stringMap);
            }

            _stringCache = cache;
        }
    }
}
