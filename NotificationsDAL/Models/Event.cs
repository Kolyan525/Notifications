using System;
using System.Collections.Generic;

namespace NotificationsDAL.Models
{
    public class Event
    {
        public long EventId { get; set; }

        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string EventLink { get; set; }
        public List<string> Category { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public DateTime StartAt { get; set; }
    }
}