using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

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
            // TODO: Check deletebehavior
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
                entity.HasOne(x => x.NotificationTypes);
            });

            modelbuilder.Entity<EventCategory>(entity =>
            {
                entity.HasKey(x => x.EventCategoryId);
                entity.Property(x => x.EventCategoryId).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Event).WithMany(x => x.EventCategories).HasForeignKey(x => x.EventId); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Category).WithMany(x => x.EventCategories).HasForeignKey(x => x.CategoryId); //.IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            // Event has ICollection<EventCategory> and ICollection<SubscriptionEvent>
            modelbuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.EventId);
                entity.Property(x => x.EventId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.SubscriptionEvents).WithOne(x => x.Event).HasForeignKey(x => x.EventId); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.EventCategories).WithOne(x => x.Event).HasForeignKey(x => x.EventId); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.CategoryId);
                entity.Property(x => x.CategoryId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.EventCategories).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            // Sub has ICollection<NotificationTypeSubscription> and virtual ICollection<SubscriptionEvent>
            modelbuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(x => x.SubscriptionId);
                entity.Property(x => x.SubscriptionId).ValueGeneratedOnAdd();
                entity.HasMany(x => x.NotificaitonTypeSubscriptions).WithOne(x => x.Subscription).HasForeignKey(x => x.SubscriptionId); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.SubscriptionEvents).WithOne(x => x.Subscription).HasForeignKey(x => x.SubscriptionId); //.IsRequired();//.OnDelete(DeleteBehavior.Cascade);
            });

            modelbuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(x => x.NotificationTypeId);
                entity.Property(x => x.NotificationTypeId).ValueGeneratedOnAdd();
            });

            // Users


            modelbuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Universal",
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Quarantine",
                }
            );

            modelbuilder.Entity<Event>().HasData(
                new Event
                {
                    EventId = 1,
                    Title = "Online Learning in NaU OA Starts",
                    Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore, an online learning will be established.",
                    ShortDesc = "Very short description for online learning",
                    EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit",
                    StartAt = DateTime.Today,
                },
                new Event
                {
                    EventId = 2,
                    Title = "International rating",
                    Description = "Congratulations, My name is Natalia, I deal with international rankings and NaUAA membership in them. This year, U - Multirank is conducting a survey among students majoring in Computer Science. Please contribute to the high place of NaUAA in this ranking by filling out a small survey.I quote the letter",
                    ShortDesc = "Very short description for international rating",
                    EventLink = "https://che-survey.de/uc/umr2022/ ",
                    StartAt = new DateTime(2021, 12, 20, 11, 24, 00),
                }
            );
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationTypeSubscription> NotificationTypeSubscription
        { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; }
    }
}
