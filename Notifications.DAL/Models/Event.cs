using System;
using System.Collections.Generic;

namespace Notifications.DAL.Models
{
    public class Event
    {
        public long EventId { get; set; }

        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string EventLink { get; set; }
        public ICollection<EventCategory>? EventCategories { get; set; }

        // ???
        public ICollection<SubscriptionEvent> SubscriptionEvents { get; set; }

        public DateTime StartAt { get; set; }
    }
}