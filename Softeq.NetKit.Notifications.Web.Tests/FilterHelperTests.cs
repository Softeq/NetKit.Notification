// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Web.Utility;
using System;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests
{
    public class FilterHelperTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void WhenStartTimeGreaterThanEndTimeThenErrorThrown()
        {
            var startTime = DateTimeOffset.UtcNow.AddHours(5);
            var endTime = DateTimeOffset.UtcNow;

            Assert.Throws<ValidationException>(() => FilterHelper.CreateOptions(startTime.ToString(), endTime.ToString()));
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("invalidFormat", "25-11-2009")]
        [InlineData("25-11-2009", "invalidFormat")]
        [InlineData("25-13-2009", "25-11-2009")]
        [InlineData("25-11-2009", "25-13-2009")]
        public void WhenFormatIsInvalidThenErrorThrown(string startTime, string endTime)
        {
            Assert.Throws<ValidationException>(() => FilterHelper.CreateOptions(startTime, endTime));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WhenFormatIsValidThenFilterReturned()
        {
            var startTime = DateTimeOffset.UtcNow;
            var endTime = DateTimeOffset.UtcNow.AddHours(4);
            var expectedStart = new DateTimeOffset(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, startTime.Second, startTime.Offset);
            var expectedEnd = new DateTimeOffset(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, endTime.Minute, endTime.Second, endTime.Offset);

            var options = FilterHelper.CreateOptions(startTime.ToString(), endTime.ToString());
            
            Assert.NotNull(options);
            Assert.Equal(expectedStart, options.StartTime);
            Assert.Equal(expectedEnd, options.EndTime);
        }
    }
}
