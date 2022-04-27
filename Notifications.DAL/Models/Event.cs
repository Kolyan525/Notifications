using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notifications.DAL.Models
{
    public class Event
    {
        public long EventId { get; set; }

        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string EventLink { get; set; }
        public ICollection<EventCategory> EventCategories { get; set; }
                // ???
        public ICollection<SubscriptionEvent> SubscriptionEvents { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartAt { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }
    }
}