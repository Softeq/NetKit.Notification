// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Service.Abstract
{
    public interface ISettingsService
    {
        Task<UserSettingsResponse> CreateUserSettingsAsync(UserProfileRequest request);
        Task<UserProfileResponse> UpdateUserSettingsAsync(UserProfileRequest request);
        Task<UserProfileResponse> GetUserSettingsAsync(UserRequest request);
        Task<NotificationSettingsResponse> GetNotificationSettingsAsync(UserRequest request);
        Task<NotificationSettingsResponse> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request);
    }
}
