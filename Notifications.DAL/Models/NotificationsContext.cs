using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Notifications.DAL.Models
{
    public class NotificationsContext : DbContext
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<SubscriptionEvent>(entity =>
            {
                entity.HasKey(x => x.SubscriptionEventId);
                entity.Property(x => x.SubscriptionEventId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Event).WithMany(x => x.SubscriptionEvents).HasForeignKey(x => x.EventId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Subscription).WithMany(x => x.SubscriptionEvents).HasForeignKey(x => x.SubscriptionId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<NotificationTypeSubscription>(entity =>
            {
                entity.HasKey(x => x.NotificaitonTypeSubscriptionId);
                entity.Property(x => x.NotificaitonTypeSubscriptionId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Subscription).WithMany(x => x.NotificaitonTypeSubscriptions).HasForeignKey(x => x.SubscriptionId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                // TODO: entity.HasOne(x => x.NotificationTypes); What relationships between them?
            });
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationTypeSubscription> NotificationTypeSubscription
        { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; }
    }
}
