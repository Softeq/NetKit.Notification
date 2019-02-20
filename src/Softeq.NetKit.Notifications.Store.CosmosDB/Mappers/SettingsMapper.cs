// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Notifications.Store.CosmosDB.Models;
using DomainUserSettings = Softeq.NetKit.Notifications.Domain.Models.NotificationSettings.UserSettings;
using DomainNotificationSetting = Softeq.NetKit.Notifications.Domain.Models.NotificationSettings.NotificationSetting;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Mappers
{
    public class SettingsMapper : Profile
    {
        public SettingsMapper()
        {
            CreateMap<UserSettings, DomainUserSettings>().ReverseMap();
            CreateMap<DomainNotificationSetting, NotificationSetting>().ReverseMap();
        }
    }
}
