using System.ComponentModel.DataAnnotations;

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
