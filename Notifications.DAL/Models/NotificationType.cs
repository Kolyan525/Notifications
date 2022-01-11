namespace Notifications.DAL.Models
{
    public class NotificationType
    {
        public long NotificationTypeId { get; set; }
        public string NotificationName { get; set; }
        // public ICollection<NotificationTypeSubscription> NotificationTypeSubscriptions { get; set; }
    }
}