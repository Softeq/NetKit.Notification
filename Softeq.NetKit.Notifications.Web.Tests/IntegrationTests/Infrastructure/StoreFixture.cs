// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Notifications.Store.Sql;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure
{
    public class StoreFixture : IDisposable
    {
        private readonly IConfiguration _configuration;

        public StoreFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();

            ClearDatabase();
            ClearCosmosDb();
        }

        private void ClearCosmosDb()
        {

        }

        private void ClearDatabase()
        {
            var connStr = _configuration["Notifications:Storage:Sql:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connStr))
            {
                return;
            }

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connStr, optionsBuilder => optionsBuilder.EnableRetryOnFailure());

            var context = new ApplicationDbContext(dbContextOptionsBuilder.Options);
            context.Database.EnsureCreated();
            context.UserSettings.RemoveRange(context.UserSettings);
            context.NotificationRecords.RemoveRange(context.NotificationRecords);
            context.RemoveRange(context.NotificationRecords);
            context.SaveChanges();
        }

        public void Dispose()
        {
            ClearDatabase();
            ClearCosmosDb();
        }
    }
}
