// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Notifications.Domain.Infrastructure
{
    public interface IUpdated
    {
        DateTimeOffset? Updated { get; set; }
    }
}
