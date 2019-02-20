// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Services.EmailNotifications.Exception
{
    public class EmailSenderException : System.Exception
    {
        public Dictionary<string, dynamic> Errors { get; }

        public EmailSenderException(string message)
            : base(message)
        {
        }

        public EmailSenderException(string message, Dictionary<string, dynamic> errors)
            : base(message)
        {
            Errors = errors;
        }

        public EmailSenderException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
