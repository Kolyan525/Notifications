using Microsoft.EntityFrameworkCore;

namespace NotificationsDAL.Models
{
    public class NotificationsContext : DbContext
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> TodoItems { get; set; }
    }
}
