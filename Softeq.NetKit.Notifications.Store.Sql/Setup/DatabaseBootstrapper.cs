// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Softeq.NetKit.Notifications.Domain.Infrastructure;

namespace Softeq.NetKit.Notifications.Store.Sql.Setup
{
    internal class DatabaseBootstrapper : IBootstrapper
    {
        private readonly ApplicationDbContext _context;

        public DatabaseBootstrapper(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task RunAsync()
        {
            return _context.Database.MigrateAsync();
        }
    }
}
