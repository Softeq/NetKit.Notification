// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Softeq.NetKit.Notifications.Store.Sql.Mappers;

namespace Softeq.NetKit.Notifications.Store.Sql.Tests
{
    public class UnitTestBase
    {
        protected readonly IMapper DefaultMapper;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public UnitTestBase()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new NotificationRecordMapper());
                cfg.AddProfile(new SettingsMapper());
            });
            DefaultMapper = mockMapper.CreateMapper();

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDB")
                .UseInternalServiceProvider(serviceProvider)
                .Options;
        }

        public ApplicationDbContext GetContext()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
