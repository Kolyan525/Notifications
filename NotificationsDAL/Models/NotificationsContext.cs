using Microsoft.EntityFrameworkCore;

namespace NotificationsDAL.Models
{
    public class NotificationsContext
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> TodoItems { get; set; }
    }
}
