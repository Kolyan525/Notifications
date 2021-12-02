namespace Notifications.DAL.Models
{
    public class SubscriptionEvent
    {
        // PK for SubscriptionEventId
        public long SubscriptionEventId { get; set; }
        // FK Reference to Subscription
        public long SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }

        // FK to Event
        public long EventId { get; set; }
        public virtual Event Event { get; set; }
    }
}