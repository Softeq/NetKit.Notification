// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Store.Sql.DataStores
{
    internal class SettingsDataStore : BaseDataStore<Models.UserSettings, UserSettings>, ISettingsDataStore
    {
        public SettingsDataStore(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<bool> DoesExistAsync(string userId)
        {
            return Context.AnyAsync(profile => profile.UserId == userId);
        }

        public async Task<UserSettings> FindAsync(string userId)
        {
            var item = await Context
                .Include(x => x.Settings)
                .AsNoTracking()
                .FirstOrDefaultAsync(settings => settings.UserId == userId);

            return item == null
                ? null
                : Mapper.Map<Models.UserSettings, UserSettings>(item);
        }

        public async Task<UserSettings> SaveAsync(UserSettings settings)
        {
            var storeEntity = Mapper.Map<UserSettings, Models.UserSettings>(settings);
            await Context.AddAsync(storeEntity);
            await SaveChangesAsync();
            return Mapper.Map<Models.UserSettings, UserSettings>(storeEntity);
        }

        public async Task<UserSettings> UpdateAsync(UserSettings settings)
        {
            var storeEntity = await Context.FindAsync(settings.Id);
            storeEntity = Mapper.Map(settings, storeEntity);
            Context.Update(storeEntity);
            await SaveChangesAsync();
            return Mapper.Map<Models.UserSettings, UserSettings>(storeEntity);
        }
    }
}
