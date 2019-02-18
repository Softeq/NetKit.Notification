// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnsureThat;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Service.Services
{
    internal class NotificationHistoryService : BaseService, INotificationHistoryService
    {
        private readonly INotificationHistoryDataStore _store;

        public NotificationHistoryService(INotificationHistoryDataStore store, 
            IMapper mapper) : base(mapper)
        {
            _store = store;
        }

        public async Task<NotificationSetResponse> ListAsync(GetNotificationsRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var set = await _store.ListAsync(request.UserId, request.PageSize, request.Options);

            var notificationResponses = set.Select(PopulateNotificationResponse).ToList();

            var result = new NotificationSetResponse
            {
                ItemsCount = notificationResponses.Count,
                Results = notificationResponses
            };

            return result;
        }

        public async Task<NotificationResponse> GetAsync(GetNotificationRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();

            var notification = await _store.FindAsync(request.NotificationId);
            if (notification == null || notification.OwnerUserId != request.UserId)
            {
                throw new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Notification record does not exist."));
            }

            var response = PopulateNotificationResponse(notification);
            return response;
        }

        public Task DeleteAllAsync(UserRequest request)
        {
            Ensure.That(request, nameof(request)).IsNotNull();
            return _store.DeleteAllAsync(request.UserId);
        }

        private NotificationResponse PopulateNotificationResponse(NotificationRecord notification)
        {
            var response = Mapper.Map<NotificationResponse>(notification);
            return response;
        }
    }
}
