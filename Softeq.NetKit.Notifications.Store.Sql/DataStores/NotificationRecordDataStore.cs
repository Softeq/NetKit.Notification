// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Store.Sql.DataStores
{
    internal class NotificationRecordDataStore : BaseDataStore<Models.NotificationRecord, NotificationRecord>, INotificationHistoryDataStore
    {
        public NotificationRecordDataStore(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<NotificationRecord> SaveAsync(NotificationRecord record)
        {
            var storeEntity = Mapper.Map<NotificationRecord, Models.NotificationRecord>(record);
            await Context.AddAsync(storeEntity);
            await SaveChangesAsync();
            return Mapper.Map<Models.NotificationRecord, NotificationRecord>(storeEntity);
        }

        public async Task<NotificationRecord> FindAsync(Guid id)
        {
            var storeEntity = await Context.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id);
            return storeEntity == null
                 ? null
                 : Mapper.Map<Models.NotificationRecord, NotificationRecord>(storeEntity);
        }

        public async Task<IEnumerable<NotificationRecord>> ListAsync(string userId, int pageSize, FilterOptions options = null)
        {
            var baseQuery = Context
                    .AsNoTracking()
                    .Where(x => x.OwnerUserId == userId);

            if (options != null)
            {
                if (options.StartTime.HasValue)
                {
                    baseQuery = baseQuery.Where(x => x.Created >= options.StartTime.Value);
                }
                if (options.EndTime.HasValue)
                {
                    baseQuery = baseQuery.Where(x => x.Created <= options.EndTime.Value);
                }
            }
          
            var storeEntities = await baseQuery.OrderByDescending(x => x.Created)
                    .Take(pageSize)
                    .ToListAsync();
            return storeEntities.Select(Mapper.Map<Models.NotificationRecord, NotificationRecord>).ToList();
        }

        public Task DeleteAllAsync(string userId)
        {
            Context.RemoveRange(Context);
            return SaveChangesAsync();
        }
    }
}
