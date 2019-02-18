// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Runtime.Serialization;
using Softeq.NetKit.Notifications.Domain.Models.Errors;

namespace Softeq.NetKit.Notifications.Domain.Exceptions
{
    [Serializable]
    public class ValidationException : ServiceException
    {
        public ValidationException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public ValidationException(string message) : base(message, new ErrorDto(ErrorCode.ValidationError, message))
        {
        }

        public ValidationException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.ValidationError, innerException.Message))
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.ValidationError, message))
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
