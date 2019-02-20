// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.EmailNotifications.Abstract;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models
{
    public class PackageArrivedEmailModel : IEmailTemplateModel
    {
        public string OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string UserName { get; set; }
    }
}
