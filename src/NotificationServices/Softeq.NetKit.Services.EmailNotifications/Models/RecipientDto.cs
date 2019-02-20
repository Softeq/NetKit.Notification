// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Services.EmailNotifications.Models
{
    public class RecipientDto
    {
        public RecipientDto(string email, string name, EmailDeliveryTypeEnum type = EmailDeliveryTypeEnum.Regular)
        {
            Email = email;
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public EmailDeliveryTypeEnum Type { get; set; }
    }
}
