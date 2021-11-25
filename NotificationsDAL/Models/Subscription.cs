using System.Collections.Generic;

namespace NotificationsDAL.Models
{
    public class Subscription
    {
        // PK for Subscription
        public long SubscriptionId { get; set; }

        // Collection of NotificaitonTypeSubscriptions
        public ICollection<NotificationTypeSubscription> NotificaitonTypeSubscriptions { get; set; }
    }
}