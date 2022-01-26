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
        readonly NotificationsContext context;
        IServiceProvider services;
        ILogger<DbInitializer> logger;
        public DbInitializer(NotificationsContext context, IServiceProvider services, ILogger<DbInitializer> logger)
        {
            this.context = context;
            this.services = services;
            this.logger = logger;
        }
        public async Task Initialize()
        {
            // TODO: update logs info placements
            await SeedEvents();
            await SeedCategories();
            await SeedSubscriptions();
            await SeedEventCategories();
            await SeedNotificationTypes();
            await SeedNotificationTypeSubscriptions();
            await SeedSubscriptionEvents();
            await SeedUsersAndRoles();

            logger.LogInformation("Finished seeding the database.");
            await context.SaveChangesAsync();
        }

        public async Task SeedEvents()
        {
            var events = new List<Event>()
            {
                new Event
                {
                    //EventId = 1,
                    Title = "Online Learning in NaU\"OA\" Starts",
                    Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.",
                    ShortDesc = "Very short description for online learning",
                    EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit",
                    StartAt = DateTime.Today,
                },
                new Event
                {
                    //EventId = 2,
                    Title = "International rating",
                    Description = "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey amongstudents majoring in Computer Science. Please contribute to the high place of Na\"OA\" in this ranking by filling out a small survey. I quote the letter below",
                    ShortDesc = "Very short description for international rating",
                    EventLink = "https://che-survey.de/uc/umr2022/ ",
                    StartAt = new DateTime(2021, 12, 20, 11, 24, 00),
                }
            };

            logger.LogInformation("Starting to seed Events");
            foreach (var evnt in events)
            {
                var ch = await context.Events.FirstOrDefaultAsync(e => e.Title == evnt.Title);
                if (ch == null)
                {
                    context.Events.Add(evnt);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedCategories()
        {
            var categories = new List<Category>()
            {
                new Category()
                {
                    CategoryId = 43,
                    CategoryName = "Universal"
                },
                new Category()
                {
                    CategoryId = 44,
                    CategoryName = "Quarantine",
                }
            };

            logger.LogInformation("Starting to seed Categories");
            foreach (var category in categories)
            {
                var ch = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);
                if (ch == null)
                {
                    context.Categories.Add(category);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedSubscriptions()
        {
            var subscriptions = new List<Subscription>()
            {
                new Subscription
                {
                    SubscriptionId = 24,
                    NotificationTypeSubscriptionId = 10,
                    SubscriptionEventId = 10,
                },
                new Subscription
                {
                    SubscriptionId = 25,
                    NotificationTypeSubscriptionId = 11,
                    SubscriptionEventId= 11,
                }
            };

            logger.LogInformation("Starting to seed Subscriptions");
            foreach (var sub in subscriptions)
            {
                var ch = await context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == sub.SubscriptionId);
                if (ch == null)
                {
                    context.Subscriptions.Add(sub);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedEventCategories()
        {
            var eventCategories = new List<EventCategory>()
            {
                new EventCategory
                {
                    //EventCategoryId = 1,
                    CategoryId = 43,
                    EventId = 53
                },
                new EventCategory
                {
                    //EventCategoryId = 2,
                    CategoryId = 44,
                    EventId = 53
                },
                new EventCategory
                {
                    //EventCategoryId = 3,
                    CategoryId = 43,
                    EventId = 54
                },
            };

            logger.LogInformation("Starting to seed EventCategories");
            foreach (var eventCategory in eventCategories)
            {
                var ch = await context.EventCategories.FirstOrDefaultAsync(ec => ec.CategoryId == eventCategory.CategoryId);
                if (ch == null)
                {
                    context.EventCategories.Add(eventCategory);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedNotificationTypes()
        {
            var notificationTypes = new List<NotificationType>()
            {
                new NotificationType
                {
                    //NotificationTypeId = 1,
                    NotificationName = "Telegram"
                },
                new NotificationType
                {
                    //NotificationTypeId = 2,
                    NotificationName = "Instagram"
                },
                new NotificationType
                {
                    //NotificationTypeId = 3,
                    NotificationName = "Discord",
                }
            };

            logger.LogInformation("Starting to seed NotificationTypes");
            foreach (var notificationType in notificationTypes)
            {
                var ch = await context.NotificationTypes.FirstOrDefaultAsync(nt => nt.NotificationName == notificationType.NotificationName);
                if (ch == null)
                {
                    context.NotificationTypes.Add(notificationType);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedNotificationTypeSubscriptions()
        {
            var notificationTypeSubscriptions = new List<NotificationTypeSubscription>()
            {
                new NotificationTypeSubscription
                {
                    //NotificaitonTypeSubscriptionId = 1,
                    NotificationTypeId = 67,
                    TelegramKey = "@Nicolas_Cage525",
                    SubscriptionId = 24,
                },
                new NotificationTypeSubscription
                {
                    //NotificaitonTypeSubscriptionId = 2,
                    NotificationTypeId = 68,
                    InstagramKey = "@DenVozniuk007",
                    SubscriptionId = 25,
                }
            };

            logger.LogInformation("Starting to seed NotificationTypeSubscriptions");
            foreach (var notificationTypeSubscription in notificationTypeSubscriptions)
            {
                var ch = await context.NotificationTypeSubscription.FirstOrDefaultAsync(nts => nts.NotificaitonTypeSubscriptionId == notificationTypeSubscription.NotificaitonTypeSubscriptionId);
                if (ch == null)
                {
                    context.NotificationTypeSubscription.Add(notificationTypeSubscription);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task SeedSubscriptionEvents()
        {
            var subscriptionEvents = new List<SubscriptionEvent>()
            {
                new SubscriptionEvent
                {
                    //SubscriptionEventId = 6,
                    EventId = 53,
                    SubscriptionId = 24
                },
                new SubscriptionEvent
                {
                    //SubscriptionEventId = 7,
                    EventId = 54,
                    SubscriptionId = 24
                },
                new SubscriptionEvent
                {
                    //SubscriptionEventId = 8,
                    EventId = 54,
                    SubscriptionId = 25
                },
            };

            logger.LogInformation("Starting to seed SubscriptionEvents");
            foreach (var subscriptionEvent in subscriptionEvents)
            {
                var ch = await context.SubscriptionEvents.FirstOrDefaultAsync(se => se.EventId == subscriptionEvent.EventId);
                if (ch == null)
                {
                    context.SubscriptionEvents.Add(subscriptionEvent);
                }
            }

            await context.SaveChangesAsync();
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