using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class ResetPasswordSmsNotification : SmsNotification
    {
        public string Token { get; set; }
    }
}
