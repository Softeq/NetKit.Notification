// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Domain.DataStores
{
    public interface ISettingsDataStore
    {
        Task<bool> DoesExistAsync(string userId);
        Task<UserSettings> FindAsync(string userId);
        Task<UserSettings> SaveAsync(UserSettings settings);
        Task<UserSettings> UpdateAsync(UserSettings settings);
    }
}
