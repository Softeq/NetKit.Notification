// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Mappers
{
    public class NotificationRecordMapper : Profile
    {
        public NotificationRecordMapper()
        {
            CreateMap<NotificationRecord, Models.NotificationRecord>().ReverseMap();
        }
    }
}
