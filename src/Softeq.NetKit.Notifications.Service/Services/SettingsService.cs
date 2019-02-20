// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using EnsureThat;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Service.Services
{
    internal class SettingsService : BaseService, ISettingsService
    {
        private readonly ISettingsDataStore _store;

        public SettingsService(ISettingsDataStore store, IMapper mapper) : base(mapper)
        {
            _store = store;
        }

        public async Task<UserSettingsResponse> CreateUserSettingsAsync(UserProfileRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settingsExist = await _store.DoesExistAsync(request.UserId);
            if (settingsExist)
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "User profile already exists."));
            }

            var profile = Mapper.Map<UserProfileRequest, UserSettings>(request);

            profile.Settings = GetDefaultSettings();

            var newProfile = await _store.SaveAsync(profile);

            return Mapper.Map<UserSettings, UserSettingsResponse>(newProfile);
        }

        public async Task<UserProfileResponse> UpdateUserSettingsAsync(UserProfileRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settings = await FindSettingsAsync(request.UserId);

            settings = Mapper.Map(request, settings);

            var updatedSettings = await _store.UpdateAsync(settings);

            return Mapper.Map<UserProfileSettings, UserProfileResponse>(updatedSettings);
        }

        public async Task<UserProfileResponse> GetUserSettingsAsync(UserRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settings = await FindSettingsAsync(request.UserId);

            return Mapper.Map<UserProfileSettings, UserProfileResponse>(settings);
        }

        public async Task<NotificationSettingsResponse> GetNotificationSettingsAsync(UserRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settings = await FindSettingsAsync(request.UserId);

            return new NotificationSettingsResponse
            {
                UserId = request.UserId,
                Settings = settings.Settings.Select(Mapper.Map<NotificationSetting, NotificationSettingModel>).ToList()
            };
        }

        public async Task<NotificationSettingsResponse> UpdateNotificationSettingsAsync(UpdateNotificationSettingsRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settings = await FindSettingsAsync(request.UserId);

            var newSettings = request.Settings.Select(Mapper.Map<NotificationSettingModel, NotificationSetting>).ToList();

            settings.Settings = PopulateSettings(newSettings);

            var updatedSettings = await _store.UpdateAsync(settings);
            return new NotificationSettingsResponse
            {
                UserId = request.UserId,
                Settings = updatedSettings.Settings.Select(Mapper.Map<NotificationSetting, NotificationSettingModel>).ToList()
            };
        }

        private async Task<UserSettings> FindSettingsAsync(string userId)
        {
            var settings = await _store.FindAsync(userId);
            Ensure.That(settings, nameof(settings),
                    options => options.WithException(new NotFoundException(new ErrorDto(ErrorCode.NotFound, "User settings do not exist."))))
                .IsNotNull();

            return settings;
        }

        private static IList<NotificationSetting> GetDefaultSettings()
        {
            var defaultSettings = new List<NotificationSetting>();
            foreach (var type in NotificationEventConfiguration.Config)
            {
                defaultSettings.AddRange(type.Value.Where(x => !x.IsMandatory).Select(x => new NotificationSetting(type.Key, x.Event)));
            }

            return defaultSettings;
        }

        private static IList<NotificationSetting> PopulateSettings(IList<NotificationSetting> newSettings)
        {
            var newSettingsMap = newSettings.GroupBy(x => x.Type).ToDictionary(setting => setting.Key, setting => setting.ToList());

            if (!ValidateSettings(newSettingsMap, out var errors))
            {
                throw new ValidationException(errors.ToArray());
            }

            return UpdateSettings(newSettingsMap);
        }

        private static IList<NotificationSetting> UpdateSettings(Dictionary<NotificationType, List<NotificationSetting>> newSettings)
        {
            var defaultSettings = GetDefaultSettings();

            foreach (var type in NotificationEventConfiguration.Config)
            {
                if (newSettings.TryGetValue(type.Key, out var newTypeSettings))
                {
                    foreach (var newSettingItem in newTypeSettings)
                    {
                        var currentSettingItem = defaultSettings.First(x => x.Event == newSettingItem.Event && x.Type == type.Key);
                        currentSettingItem.Enabled = newSettingItem.Enabled;
                    }
                }
            }

            return defaultSettings;
        }

        private static bool ValidateSettings(Dictionary<NotificationType, List<NotificationSetting>> newSettings, out IList<ErrorDto> errors)
        {
            errors = new List<ErrorDto>();

            foreach (var typeSettings in newSettings)
            {
                var defaultSettings = NotificationEventConfiguration.Config[typeSettings.Key];
                var unsupported = typeSettings.Value.Select(x => x.Event).Except(defaultSettings.Where(x => !x.IsMandatory).Select(x => x.Event));
                if (unsupported.Any())
                {
                    var validEvents = string.Join(", ", typeSettings.Value.Select(x => x.ToString()));
                    errors.Add(new ErrorDto(ErrorCode.ValidationError, $"Unsupported event for notification type {typeSettings.Key}. Supported events are: {validEvents}"));
                }
            }

            return !errors.Any();
        }
    }
}
