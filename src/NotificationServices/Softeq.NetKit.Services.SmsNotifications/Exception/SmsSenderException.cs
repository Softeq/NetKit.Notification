// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Services.SmsNotifications.Exception
{
    public class SmsSenderException : System.Exception
    {
        public Dictionary<string, dynamic> Errors { get; }

        public SmsSenderException(string message)
            : base(message)
        {
        }

        public SmsSenderException(string message, Dictionary<string, dynamic> errors)
            : base(message)
        {
            Errors = errors;
        }

        public SmsSenderException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
