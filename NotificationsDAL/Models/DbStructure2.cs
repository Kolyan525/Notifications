using System;
using System.Collections.Generic;

namespace NotificationsDAL
{
    public class Role
    {

    }

    public class ApplicationUser
    {
        // Standard IdentityID
        // 
        public ICollection<Role> Roles { get; set; }
    }

    public class NotificationType
    {
        // Name of the category?
        public string NotificationName { get; set; }
    }

    public class NotificaitonTypeSubscription
    {
        public long NotificationTypeId { get; set; }
        public virtual NotificationType NotificationType { get; set; }

        public long SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }

        public string TelegramKey { get; set; }
        public string DiscordKey { get; set; }
        public string InstagramKey { get; set; }
    }

    public class Subscription
    {
        public long SubscriptionId { get; set; }
        public ICollection<NotificaitonTypeSubscription> NotificaitonTypeSubscriptions { get; set; }
    }

    public class SubscriptionEvent
    {
        public long SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }

        public long EventId { get; set; }
        public virtual Event Event { get; set; }
    }

    public class Event
    {
        public long EventId { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public DateTime StartAt { get; set; }
    }
}