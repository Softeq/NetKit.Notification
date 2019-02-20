// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using AspNetCore.AsyncInitialization;
using Softeq.NetKit.Notifications.Domain.Infrastructure;

namespace Softeq.NetKit.Notifications.Web.Infrastructure
{
    internal class StorageInitializer : IAsyncInitializer
    {
        private readonly IBootstrapper _bootstrapper;

        public StorageInitializer(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public Task InitializeAsync()
        {
            return _bootstrapper.RunAsync();
        }
    }
}
