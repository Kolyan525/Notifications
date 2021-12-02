using Microsoft.AspNetCore.Identity;

namespace Notifications.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Standard IdentityID
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}