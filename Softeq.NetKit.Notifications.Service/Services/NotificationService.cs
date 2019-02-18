// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.Services
{
    internal class NotificationService : BaseService, INotificationService
    {
        private readonly ISettingsDataStore _settingsStore;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly INotificationHistoryDataStore _historyStore;

        public NotificationService(
            ISettingsDataStore settingsStore,
            IEnumerable<INotificationSender> senders,
            INotificationHistoryDataStore historyStore,
            IMapper mapper) : base(mapper)
        {
            _settingsStore = settingsStore;
            _senders = senders;
            _historyStore = historyStore;
        }

        public async Task<SendNotificationResponse> PostAsync(SendNotificationRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var settings = await _settingsStore.FindAsync(request.RecipientUserId);
            if (settings == null)
            {
                throw new NotFoundException(new ErrorDto(ErrorCode.NotFound, "User settings do not exist."));
            }

            var message = Mapper.Map<NotificationMessage>(request);

            var response = new SendNotificationResponse();

            foreach (var sender in _senders)
            {
                var senderResult = await sender.SendAsync(message, settings);
                if (senderResult.Status != NotificationSendingStatus.Skipped)
                {
                    response.Results.Add(senderResult);
                }
            }

            if (response.Results.Any(x => x.Status == NotificationSendingStatus.Success))
            {
                var newRecord = Mapper.Map<NotificationRecord>(request);
                newRecord.UserSettingsId = settings.Id;

                var record = await _historyStore.SaveAsync(newRecord);
                response.NotificationRecordId = record.Id;
            }

            return response;
        }
    }
}
