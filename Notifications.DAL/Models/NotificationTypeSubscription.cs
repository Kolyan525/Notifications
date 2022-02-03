using System.ComponentModel.DataAnnotations;

namespace Notifications.DAL.Models
{
    public class NotificationTypeSubscription
    {
        // PK for NotificaitonTypeSubscription
        [Key]
        public long NotificaitonTypeSubscriptionId { get; set; }

        // FK for NotificationType
        public long NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }

        // FK for Subscription
        public long SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }

        public string TelegramKey { get; set; }
        public string InstagramKey { get; set; }
        public string DiscordKey { get; set; }
    }
}