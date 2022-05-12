using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notifications.DAL.Models.Telegram;

namespace Notifications.DAL.Models
{
    public class NotificationsContext : IdentityDbContext<ApplicationUser>
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // TODO: Check DeleteBehavior
            modelbuilder.Entity<SubscriptionEvent>(entity =>
            {
                entity.HasKey(x => x.SubscriptionEventId);
                entity.Property(x => x.SubscriptionEventId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Event).WithMany(x => x.SubscriptionEvents).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.SetNull); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Subscription).WithMany(x => x.SubscriptionEvents).OnDelete(DeleteBehavior.Cascade); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<NotificationTypeSubscription>(entity =>
            {
                entity.HasKey(x => x.NotificaitonTypeSubscriptionId);
                entity.Property(x => x.NotificaitonTypeSubscriptionId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Subscription).WithMany(x => x.NotificationTypeSubscriptions).OnDelete(DeleteBehavior.Cascade); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.NotificationType);
            });

            modelbuilder.Entity<EventCategory>(entity =>
            {
                entity.HasKey(x => x.EventCategoryId);
                entity.Property(x => x.EventCategoryId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Event).WithMany(x => x.EventCategories).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.SetNull); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Category).WithMany(x => x.EventCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            // Event has ICollection<EventCategory> and ICollection<SubscriptionEvent>
            modelbuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.EventId);
                entity.Property(x => x.EventId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.SubscriptionEvents).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.EventCategories).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.CategoryId);
                entity.Property(x => x.CategoryId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.EventCategories).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            // Sub has ICollection<NotificationTypeSubscription> and virtual ICollection<SubscriptionEvent>
            modelbuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(x => x.SubscriptionId);
                entity.Property(x => x.SubscriptionId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.NotificationTypeSubscriptions).WithOne(x => x.Subscription).HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Cascade); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.SubscriptionEvents).WithOne(x => x.Subscription).HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.Cascade); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(x => x.NotificationTypeId);
                entity.Property(x => x.NotificationTypeId).ValueGeneratedOnAdd();
            });
        }

        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationTypeSubscription> NotificationTypeSubscription { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; }
        public DbSet<TelegramUser> Users { get; set; }
        public DbSet<EventActionActive> telegramEvent { get; set; }
    }
}
