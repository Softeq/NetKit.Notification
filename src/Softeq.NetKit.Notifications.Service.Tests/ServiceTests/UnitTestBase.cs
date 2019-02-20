// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Softeq.NetKit.Notifications.Service.Mappers;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests
{
    public class UnitTestBase
    {
        protected readonly IMapper DefaultMapper;

        public UnitTestBase()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new NotificationRecordMapper());
                cfg.AddProfile(new SettingsMapper());
                cfg.AddProfile(new NotificationMapper());
            });
            DefaultMapper = mockMapper.CreateMapper();
        }
    }
}
