// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;

namespace Softeq.NetKit.Notifications.Service.Mappers
{
    public class NotificationRecordMapper : Profile
    {
        public NotificationRecordMapper()
        {
            CreateMap<SendNotificationRequest, NotificationRecord>()
                .ForMember(x => x.OwnerUserId, expression => expression.MapFrom(request => request.RecipientUserId))
                .ForMember(x => x.Event, expression => expression.MapFrom(request => request.EventType));

            CreateMap<SendNotificationRequest, NotificationMessage>()
                .ForMember(x=>x.Event, expression => expression.MapFrom(request => request.EventType));
        }
    }
}
