// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility
{
    public static class IdAssertions
    {
        public static void BeValidNotificationId(this ReferenceTypeAssertions<string, StringAssertions> assertions, string because = " ", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(IsValidId(assertions.Subject))
                .BecauseOf(because, becauseArgs)
                .FailWith("Notification id [{0}] should not be null, empty, whitespace or equal to empty GUID and should be a valid GUID", assertions.Subject);
        }

        private static bool IsValidId(string notificationId)
        {
            // id should not be empty, null or whitespace.
            if (string.IsNullOrWhiteSpace(notificationId))
            {
                return false;
            }

            // Id should be valid GUID
            if (!Guid.TryParse(notificationId, out var parsedId))
            {
                return false;
            }

            // Id should not be equal to empty GUID
            if (parsedId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
