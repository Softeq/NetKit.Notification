// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Infrastructure;

namespace Softeq.NetKit.Notifications.Store.Sql.DataStores
{
    internal abstract class BaseDataStore<TEntityModel, TResultModel> 
        where TEntityModel : class, new()
        where TResultModel : class, new()
    {
        protected IMapper Mapper;
        protected DbSet<TEntityModel> Context;
        private readonly DbContext _dbContext;

        protected BaseDataStore(ApplicationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            Context = context.Set<TEntityModel>();
            Mapper = mapper;
        }

        protected Task SaveChangesAsync()
        {
            AddTimestamps();
            return _dbContext.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entitiesAdded = _dbContext.ChangeTracker.Entries().Where(x => x.Entity is ICreated && x.State == EntityState.Added);
            foreach (var entity in entitiesAdded)
            {
                ((ICreated)entity.Entity).Created = DateTimeOffset.UtcNow;
            }

            var entitiesModified = _dbContext.ChangeTracker.Entries().Where(x => x.Entity is IUpdated && x.State == EntityState.Modified);
            foreach (var entity in entitiesModified)
            {
                ((IUpdated)entity.Entity).Updated = DateTimeOffset.UtcNow;
            }
        }
    }
}
