// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using EnsureThat;

namespace Softeq.NetKit.Notifications.Service.Services
{
    public class BaseService
    {
        protected readonly IMapper Mapper;

        protected BaseService(IMapper mapper)
        {
            Ensure.That(mapper, nameof(mapper)).IsNotNull();

            Mapper = mapper;
        }
    }
}
