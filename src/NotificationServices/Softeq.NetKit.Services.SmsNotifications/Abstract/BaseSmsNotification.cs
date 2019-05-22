﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Services.SmsNotifications.Abstract
{
    public class BaseSmsNotification : ISmsNotification
    {
        public string RecipientPhoneNumber { get; set; }
        public string Text { get; set; }
    }
}
