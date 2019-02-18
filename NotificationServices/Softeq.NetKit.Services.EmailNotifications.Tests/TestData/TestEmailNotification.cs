// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications.Tests.TestData
{
    public class TestEmailNotification : BaseEmailNotification<TestEmailModel>
    {
        public TestEmailNotification(string toEmail, string toName, TestEmailModel model) : base(new RecipientDto(toEmail, toName))
        {
            TemplateModel = model;
            Subject = "Test Email";
            HtmlTemplate = "TestEmail.html";
            BaseHtmlTemplate = "BaseEmail.html";
        }
    }
}
