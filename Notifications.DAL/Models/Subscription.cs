using System.Collections.Generic;

namespace Notifications.DAL.Models
{
    public class Subscription
    {
        // PK for Subscription
        public long SubscriptionId { get; set; }

        // Collection of NotificaitonTypeSubscriptions
        public ICollection<NotificationTypeSubscription> NotificaitonTypeSubscriptions { get; set; }
        public virtual ICollection<SubscriptionEvent> SubscriptionEvents { get; set; }
    }
}