// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push;

namespace Softeq.NetKit.Notifications.Service.Services
{
    internal class PushNotificationSubscriptionService : IPushNotificationSubscriptionService
    {
        private readonly ISettingsDataStore _settingsStore;
        private readonly IPushNotificationSubscriber _pushSubscriber;

        public PushNotificationSubscriptionService(
            ISettingsDataStore settingsStore,
            IPushNotificationSubscriber pushSubscriber)
        {
            _settingsStore = settingsStore;
            _pushSubscriber = pushSubscriber;
        }

        public async Task UnsubscribeDeviceAsync(PushDeviceRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            await CheckUserProfile(request.UserId);

            await _pushSubscriber.UnsubscribeDeviceAsync(request.DeviceToken);
        }

        public async Task UnsubscribeUserAsync(UserRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            await CheckUserProfile(request.UserId);

            await _pushSubscriber.UnsubscribeByTagAsync(TagHelper.GetUserTag(request.UserId));
        }

        public async Task CreateOrUpdateSubscriptionAsync(PushDeviceRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            await CheckUserProfile(request.UserId);

            var tags = new List<string> { TagHelper.GetUserTag(request.UserId) };
            await _pushSubscriber.CreateOrUpdatePushSubscriptionAsync(new PushSubscriptionRequest(request.DeviceToken, (PushPlatformEnum)request.Platform, tags));
        }

        private async Task CheckUserProfile(string userId)
        {
            var profileExists = await _settingsStore.DoesExistAsync(userId);
            if (!profileExists)
            {
                throw new NotFoundException(new ErrorDto(ErrorCode.NotFound, "User settings do not exist."));
            }
        }
    }
}
