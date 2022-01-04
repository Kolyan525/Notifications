using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DTO.DTOs
{
    public class CreateNotificationTypeDTO
    {
        [Required]
        [StringLength(maximumLength: 20, ErrorMessage = "Notification type name is too long")]
        public string NotificationName { get; set; }
    }
    public class NotificationTypeDTO : CreateNotificationTypeDTO
    {
        public long NotificationTypeId { get; set; }
    }
}
