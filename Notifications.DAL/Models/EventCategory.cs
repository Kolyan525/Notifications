using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Models
{
    public class EventCategory
    {
        public long EventCategoryId { get; set; }

        public long EventId { get; set; }
        public virtual Event Event { get; set; }

        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
