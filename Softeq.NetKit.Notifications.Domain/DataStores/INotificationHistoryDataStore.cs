// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;

namespace Softeq.NetKit.Notifications.Domain.DataStores
{
    public interface INotificationHistoryDataStore
    {
        Task<NotificationRecord> SaveAsync(NotificationRecord record);
        Task<NotificationRecord> FindAsync(Guid id);
        Task<IEnumerable<NotificationRecord>> ListAsync(string userId, int pageSize, FilterOptions options = null);
        Task DeleteAllAsync(string userId);
    }
}
