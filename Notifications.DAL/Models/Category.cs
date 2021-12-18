using System.Collections.Generic;

namespace Notifications.DAL.Models
{
    public class Category
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<EventCategory> EventCategories { get; set; }
    }
}