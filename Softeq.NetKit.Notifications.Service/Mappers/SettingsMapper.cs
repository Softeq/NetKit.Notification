// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;

namespace Softeq.NetKit.Notifications.Service.Mappers
{
    public class SettingsMapper : Profile
    {
        public SettingsMapper()
        {
            CreateMap<NotificationSetting, NotificationSettingModel>();
            CreateMap<UserProfileSettings, UserProfileResponse>();
            CreateMap<UserSettings, UserSettingsResponse>();
            CreateMap<UserProfileRequest, UserSettings>();
        }
    }
}
