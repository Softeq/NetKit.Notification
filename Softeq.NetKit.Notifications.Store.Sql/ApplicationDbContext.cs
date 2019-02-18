// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.EntityFrameworkCore;
using Softeq.NetKit.Notifications.Store.Sql.Mappings;
using Softeq.NetKit.Notifications.Store.Sql.Models;

namespace Softeq.NetKit.Notifications.Store.Sql
{
    public class ApplicationDbContext : DbContext
    {
        //public ApplicationDbContext()
        //{
        //}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<NotificationRecord> NotificationRecords{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Uncomment during migration
            //optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=NotificationsDb;Integrated Security=True;", builder => builder.EnableRetryOnFailure());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //put db configuration here
            base.OnModelCreating(builder);

            builder.AddEntityConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
