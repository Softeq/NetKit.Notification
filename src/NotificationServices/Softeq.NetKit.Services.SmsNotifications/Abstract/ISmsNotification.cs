// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Services.SmsNotifications.Abstract
{
    public interface ISmsNotification
    {
        IEnumerable<string> RecipientPhoneNumbers { get; set; }
        string Text { get; set; }
    }
}
