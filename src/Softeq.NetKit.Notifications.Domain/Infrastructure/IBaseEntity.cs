// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Notifications.Domain.Infrastructure
{
    public interface IBaseEntity<T> : IEntity
    {
        T Id { get; set; }
    }
}
