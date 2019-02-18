// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications.Tests.TestData
{
    public class TemplatedTestEmailNotification : BaseEmailNotification<TestEmailModel>
    {
        public TemplatedTestEmailNotification(string toEmail, string toName, TestEmailModel model) : base(new RecipientDto(toEmail, toName))
        {
            TemplateModel = model;
            Subject = "Message is sent to {0}";
            HtmlTemplate = "TestEmail.html";
            BaseHtmlTemplate = "BaseEmail.html";
        }

        public override string FormatSubject()
        {
            return string.Format(Subject, TemplateModel.Name);
        }
    }
}
