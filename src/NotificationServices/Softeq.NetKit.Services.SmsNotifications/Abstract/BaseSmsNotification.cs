// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Services.SmsNotifications.Abstract
{
    public class BaseSmsNotification : ISmsNotification
    {
        public IEnumerable<string> RecipientPhoneNumbers { get; set; }
        public string Text { get; set; }
    }
}
