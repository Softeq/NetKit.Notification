// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Services.EmailNotifications.Models
{
    public class SendEmailDto
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public IEnumerable<RecipientDto> Recipients { get; set; }
		public string Subject { get; set; }
        public string Text { get; set; }
        public string HtmlText { get; set; }
    }
}
