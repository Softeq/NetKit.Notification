// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Security.Claims;
using IdentityModel;
using Serilog.Core;
using Serilog.Events;

namespace Softeq.NetKit.Notifications.Web.Infrastructure.Logging
{
    internal class UserContextEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (ClaimsPrincipal.Current != null)
            {
                var userId = ClaimsPrincipal.Current.FindFirstValue(JwtClaimTypes.Subject); ;
                var correlationIdProperty = new LogEventProperty("UserId", new ScalarValue(userId ?? "unknown"));

                logEvent.AddPropertyIfAbsent(correlationIdProperty);
            }
        }
    }
}
