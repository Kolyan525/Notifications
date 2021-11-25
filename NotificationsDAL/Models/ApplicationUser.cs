using Microsoft.AspNetCore.Identity;

namespace NotificationsDAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Standard IdentityID
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}