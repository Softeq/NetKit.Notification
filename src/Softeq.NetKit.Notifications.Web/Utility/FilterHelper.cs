// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using System;
using System.Globalization;

namespace Softeq.NetKit.Notifications.Web.Utility
{
    internal static class FilterHelper
    {
        public const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ssK";

        public static FilterOptions CreateOptions(string startTime, string endTime)
        {
            var startTimeOffsetParam = Parse(startTime, nameof(startTime));
            var endTimeOffsetParam = Parse(endTime, nameof(endTime));

            if (startTimeOffsetParam.HasValue && endTimeOffsetParam.HasValue && startTimeOffsetParam > endTimeOffsetParam)
                throw new ValidationException(new ErrorDto(ErrorCode.ValidationError, "Start time is greater than end time"));

            return new FilterOptions(startTimeOffsetParam, endTimeOffsetParam);
        }

        private static DateTimeOffset? Parse(string paramValue, string paramName)
        {
            if (!string.IsNullOrEmpty(paramValue))
            {
                if (!DateTimeOffset.TryParse(paramValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedValue))
                    throw new ValidationException(new ErrorDto(ErrorCode.ValidationError, $"Invalid {paramName} parameter format. Valid format is {DateTimeFormat}"));
                return parsedValue;
            }

            return null;
        }
    }
}
