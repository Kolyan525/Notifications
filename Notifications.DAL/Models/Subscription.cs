using System.Collections.Generic;

namespace Notifications.DAL.Models
{
    public class Subscription
    {
        // PK for Subscription
        public long SubscriptionId { get; set; }

        // Collection of NotificaitonTypeSubscriptions
        public long NotificationTypeSubscriptionId { get; set; }
        public ICollection<NotificationTypeSubscription> NotificationTypeSubscriptions { get; set; }

        public long SubscriptionEventId { get; set; }
        public ICollection<SubscriptionEvent> SubscriptionEvents { get; set; }
    }
}