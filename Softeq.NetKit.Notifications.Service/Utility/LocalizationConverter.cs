// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Service.Utility
{
    internal static class LocalizationConverter
    {
        private const string en = "en-US";
        private const string ru = "ru-RU";
        private const string fr = "fr-FR";

        public static string GetLanguageCode(this LanguageName name)
        {
            switch (name)
            {
                case LanguageName.En:
                    return en;
                case LanguageName.Fr:
                    return fr;
                case LanguageName.Ru:
                    return ru;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
        }
    }
}
