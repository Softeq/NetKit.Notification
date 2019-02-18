// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response
{
    public class UserProfileResponse
    {
        public string UserId { get; set; }
        public LanguageName Language { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
