using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.DAL.DbInitializer
{
    public class DbInitializer
    {
        private readonly NotificationsContext context;
        private readonly IServiceProvider services;
        private readonly ILogger<DbInitializer> logger;

        public DbInitializer(NotificationsContext context, IServiceProvider services, ILogger<DbInitializer> logger)
        {
            this.context = context;
            this.services = services;
            this.logger = logger;
        }
        public async Task Initialize()
        {
            // TODO: update logs info placements
            await SeedCategories();
            await SeedEvents();
            //await SeedCategories();
            //await SeedEventCategories();
            //await SeedSubscriptions();
            //await SeedNotificationTypes();
            //await SeedNotificationTypeSubscriptions();
            //await SeedSubscriptionEvents();
            await SeedUsersAndRoles();

            logger.LogInformation("Finished seeding the database.");
            await context.SaveChangesAsync();
        }
        // TODO: redo seeder logic on related entities

        public async Task SeedCategories()
        {
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Universal"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Quarantine"
            });

            await context.SaveChangesAsync();
        }

        public async Task SeedEvents()
        {
            var categoryUniversal = context.Categories.First(x => x.CategoryName == "Universal");
            var categoryQuarantine = context.Categories.First(x => x.CategoryName == "Quarantine");

            await CreateEventIfNotExists(new Event
            {
                Title = "Online Learning in NaU\"OA\" Starts",
                Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.",
                ShortDesc = "Very short description for online learning",
                EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit",
                StartAt = DateTime.Today,
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    },
                    new EventCategory
                    {
                        Category = categoryQuarantine
                    }
                }
            });

            await CreateEventIfNotExists(new Event
            {
                Title = "International rating",
                Description = "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey among students majoring in Computer Science. Please contribute to the high place of NaU\"OA\" in this ranking by filling out a small survey. I pinned the letter below",
                ShortDesc = "Very short description for international rating",
                EventLink = "https://che-survey.de/uc/umr2022/ ",
                StartAt = new DateTime(2021, 12, 20, 11, 24, 00),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    }
                }
            });

            await context.SaveChangesAsync();

            // Object reference not set to an instance of an object, EventCategories 

            //var ec = await CreateEventCategoryIfNotExists(new EventCategory
            //{
            //    Event = evnt1,
            //    Category = cat1
            //});
            //var ec1 = await CreateEventCategoryIfNotExists(new EventCategory
            //{
            //    Event = evnt1,
            //    Category = cat2
            //});
            //var ec2 = await CreateEventCategoryIfNotExists(new EventCategory
            //{
            //    Event = evnt2,
            //    Category = cat1
            //});

            ////SubEvents, notif types, nts, subs

            //var nt = await CreateNotificationTypeIfNotExists("Telegram");
            //var nt1 = await CreateNotificationTypeIfNotExists("Viber");
            //var nt2 = await CreateNotificationTypeIfNotExists("Discord");


            //var nts = await CreateNotificationTypeSubscriptionIfNotExists(new NotificationTypeSubscription
            //{
            //    NotificationType = nt,
            //    TelegramKey = "@Nicolas_Cage525",
            //});
            //var nts1 = await CreateNotificationTypeSubscriptionIfNotExists(new NotificationTypeSubscription
            //{
            //    NotificationType = nt2,
            //    TelegramKey = "Den1337",
            //});

            //var subevnt = await CreateSubscriptionEventIfNotExists(new SubscriptionEvent
            //{
            //    Event = evnt1,
            //});
            //var subevnt1 = await CreateSubscriptionEventIfNotExists(new SubscriptionEvent
            //{
            //    Event = evnt2,
            //});
            //var subevnt2 = await CreateSubscriptionEventIfNotExists(new SubscriptionEvent
            //{
            //    Event = evnt2,
            //});

            //var sub1 = await CreateSubscriptionIfNotExists(new Subscription
            //{
            //    NotificationTypeSubscriptions = new List<NotificationTypeSubscription>() { nts, nts1 },
            //    SubscriptionEvents = new List<SubscriptionEvent>() { subevnt, subevnt1 }
            //});
            //var sub2 = await CreateSubscriptionIfNotExists(new Subscription
            //{
            //    NotificationTypeSubscriptions = new List<NotificationTypeSubscription>() { nts },
            //    SubscriptionEvents = new List<SubscriptionEvent>() { subevnt1, subevnt2 }
            //});
        }

        public async Task CreateEventIfNotExists(Event vent)
        {
            var @event = await context.Events.FirstOrDefaultAsync(
                e => e.Title == vent.Title &&
                e.Description == vent.Description &&
                e.StartAt == vent.StartAt);
            if (@event == null)
            {
                await context.Events.AddAsync(vent);
            }

            // possible, we need to have a logic of how to update an existing event
        }

        public async Task CreateCategoryIfNotExists(Category category)
        {
            var cat = await context.Categories.FirstOrDefaultAsync(e => e.CategoryName == category.CategoryName);
            if (cat == null)
            {
                var newCategory = new Category
                {
                    CategoryName = category.CategoryName
                };

                await context.Categories.AddAsync(newCategory);
            }
        }

        public async Task<EventCategory> CreateEventCategoryIfNotExists(EventCategory eventCategory)
        {
            var ec = await context.EventCategories.FirstOrDefaultAsync(e => e.Category == eventCategory.Category && e.Event == eventCategory.Event);
            if (ec == null)
            {
                var newEventCategory = new EventCategory
                {
                    Event = eventCategory.Event,
                    Category = eventCategory.Category,
                };

                await context.EventCategories.AddAsync(newEventCategory);
                return newEventCategory;
            }
            return ec;
        }

        public async Task<SubscriptionEvent> CreateSubscriptionEventIfNotExists(SubscriptionEvent subscriptionEvent)
        {
            var se = await context.SubscriptionEvents.FirstOrDefaultAsync(e => e.Event == subscriptionEvent.Event && e.Subscription == subscriptionEvent.Subscription);
            if (se == null)
            {
                var newSubscriptionEvent = new SubscriptionEvent
                {
                    Event = subscriptionEvent.Event,
                    Subscription = subscriptionEvent.Subscription
                };

                await context.SubscriptionEvents.AddAsync(newSubscriptionEvent);
                return newSubscriptionEvent;
            }
            return se;
        }

        public async Task<NotificationType> CreateNotificationTypeIfNotExists(string notificationTypeName)
        {
            var nt = await context.NotificationTypes.FirstOrDefaultAsync(n => n.NotificationName == notificationTypeName);
            if (nt == null)
            {
                var newNotificationType = new NotificationType
                {
                    NotificationName = notificationTypeName
                };

                await context.NotificationTypes.AddAsync(newNotificationType);
                return newNotificationType;
            }
            return nt;
        }

        public async Task<NotificationTypeSubscription> CreateNotificationTypeSubscriptionIfNotExists(NotificationTypeSubscription notification)
        {
            var nts = await context.NotificationTypeSubscription.FirstOrDefaultAsync(n => n.NotificationType == notification.NotificationType
                            && n.TelegramKey == notification.TelegramKey && n.InstagramKey == notification.InstagramKey && n.DiscordKey == notification.DiscordKey && n.Subscription == notification.Subscription);

            if (nts == null)
            {
                var newNotificationTypeSubscription = new NotificationTypeSubscription
                {
                    NotificationType = notification.NotificationType,
                    TelegramKey = notification.TelegramKey,
                    InstagramKey = notification.InstagramKey,
                    DiscordKey = notification.DiscordKey,
                    Subscription = notification.Subscription
                };

                await context.NotificationTypeSubscription.AddAsync(newNotificationTypeSubscription);
                return newNotificationTypeSubscription;
            }
            return nts;
        }

        public async Task<Subscription> CreateSubscriptionIfNotExists(Subscription subscription)
        {
            var sub = await context.Subscriptions.FirstOrDefaultAsync(s => s.NotificationTypeSubscriptions == subscription.NotificationTypeSubscriptions 
                && s.SubscriptionEvents == subscription.SubscriptionEvents);
            if (sub == null)
            {
                var newSubscription = new Subscription
                {
                    NotificationTypeSubscriptions = subscription.NotificationTypeSubscriptions,
                    SubscriptionEvents = subscription.SubscriptionEvents
                };

                await context.Subscriptions.AddAsync(newSubscription);
                return newSubscription;
            }
            return sub;
        }

        public async Task SeedUsersAndRoles()
        {
            string[] roles = new string[] { "Admin", "Manager" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    logger.LogInformation("Starting to seed Roles");
                    await roleStore.CreateAsync(new IdentityRole(role));
                }
            }

            UserManager<ApplicationUser> userManager = services.GetService<UserManager<ApplicationUser>>();

            if (userManager.FindByEmailAsync("mykola.kalinichenko@oa.edu.ua").Result == null && userManager.FindByEmailAsync("denys.vozniuk@oa.edu.ua").Result == null)
            {
                logger.LogInformation("Starting to seed Users");
                // https://stackoverflow.com/questions/50785009/how-to-seed-an-admin-user-in-ef-core-2-1-0

                ApplicationUser admin = new ApplicationUser
                {
                    UserName = "Kolyan525",
                    FirstName = "Kolya",
                    LastName = "Kalina",
                    Email = "mykola.kalinichenko@oa.edu.ua",
                    EmailConfirmed = true
                };

                ApplicationUser manager = new ApplicationUser
                {
                    UserName = "Denys",
                    FirstName = "Denys",
                    LastName = "Vozniuk",
                    Email = "denys.vozniuk@oa.edu.ua",
                    EmailConfirmed = true
                };

                IdentityResult adminResult = userManager.CreateAsync(admin, "Kolk@1337").Result;
                IdentityResult managerResult = userManager.CreateAsync(admin, "Den@1337").Result;

                if (adminResult.Succeeded && managerResult.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                    userManager.AddToRoleAsync(manager, "Manager").Wait();
                }
                else
                {
                    logger.LogInformation("User seeding failed!");
                }
            }
        }
    }
}