// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications.Abstract
{
    public interface IEmailNotification
    {
        string Subject { get; set; }
        IEnumerable<RecipientDto> Recipients { get; set; }
        string Text { get; set; }
        string BaseHtmlTemplate { get; set; }
        string HtmlTemplate { get; set; }
		string GetHtml();
        string FormatSubject();
    }
}
