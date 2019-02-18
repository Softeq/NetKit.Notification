// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Domain.Infrastructure
{
    public interface IBootstrapper
    {
        Task RunAsync();
    }
}
