// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Softeq.NetKit.Notifications.Domain.Models.Errors;

namespace Softeq.NetKit.Notifications.Domain.Exceptions
{
    [Serializable]
    public class ConflictException : ServiceException
    {
        public ConflictException(params ErrorDto[] errors)
        {
            InitializeErrors(errors);
        }

        public ConflictException(string message) : base(message, new ErrorDto(ErrorCode.ConflictError, message))
        {
        }

        public ConflictException(Exception innerException) : base("See inner exception.", innerException, new ErrorDto(ErrorCode.NotFound, innerException.Message))
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException, new ErrorDto(ErrorCode.NotFound, message))
        {
        }

        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
