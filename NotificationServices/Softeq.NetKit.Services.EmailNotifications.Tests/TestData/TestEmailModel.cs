// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.EmailNotifications.Abstract;

namespace Softeq.NetKit.Services.EmailNotifications.Tests.TestData
{
    public class TestEmailModel : IEmailTemplateModel
    {
        public TestEmailModel(string link, string name)
        {
            Link = link;
            Name = name;
        }

        public string Link { get; set; }

        public string Name { get; set; }
    }
}
