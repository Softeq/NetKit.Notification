// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Services.SmsNotifications.Abstract
{
    public interface ISmsNotification
    {
        string Recipient { get; set; }
        string Text { get; set; }
    }
}
