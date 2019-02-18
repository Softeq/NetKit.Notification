// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Infrastructure;

namespace Softeq.NetKit.Notifications.Domain.Models
{
    public class Entity<T> : IBaseEntity<T>
    {
        public virtual T Id { get; set; }
    }
}
