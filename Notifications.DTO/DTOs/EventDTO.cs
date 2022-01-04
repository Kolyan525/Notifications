using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DTO.DTOs
{
    public class CreateEventDTO
    {
        [Required]
        [StringLength(maximumLength: 20, ErrorMessage = "Event title is too long")]
        public string Title { get; set; }

        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Short description is too long")]
        public string ShortDesc { get; set; }

        [Required]
        [StringLength(maximumLength: 500, ErrorMessage = "Description is too long")]
        public string Description { get; set; }

        //[Required]
        //[StringLength(maximumLength: 50, ErrorMessage = "Country Name is too long")]
        public string EventLink { get; set; }

        // Required
        public DateTime StartAt { get; set; }
    }
    public class EventDTO : CreateEventDTO
    {
        public long EventId { get; set; }

        // These should refer to DTO as well
        public ICollection<EventCategory> EventCategories { get; set; }
        public ICollection<SubscriptionEvent> SubscriptionEvents { get; set; }
    }
}
