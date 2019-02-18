// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Notifications.Domain.Models
{
    public class FilterOptions
    {
        public DateTimeOffset? StartTime { get; }
        public DateTimeOffset? EndTime { get; }

        public FilterOptions(DateTimeOffset? startTime = null, DateTimeOffset? endTime = null)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
