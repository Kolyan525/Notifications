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
        public static void Initialize(NotificationsContext context, IServiceProvider services)
        {
            // Get a logger
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();

            // Make sure the database is created
            //context.Database.EnsureCreated();

            // context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            // context.Database.Migrate();

            // TODO: Edit checks
            if (context.Events.Any() && context.Categories.Any() && context.Roles.Any())
            {
                logger.LogInformation("The database was already seeded");
                return;
            }
             
            logger.LogInformation("Starting to seed database.");

            //var events = new List<Event>()
            //{
            //    new Event
            //    {
            //        //EventId = 1,
            //        Title = "Online Learning in NaU\"OA\" Starts",
            //        Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.",
            //        ShortDesc = "Very short description for online learning",
            //        EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit",
            //        StartAt = DateTime.Today,
            //    },
            //    new Event
            //    {
            //        //EventId = 2,
            //        Title = "International rating",
            //        Description = "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey amongstudents majoring in Computer Science. Please contribute to the high place of Na\"OA\" in this ranking by filling out a small survey. I quote the letter below",
            //        ShortDesc = "Very short description for international rating",
            //        EventLink = "https://che-survey.de/uc/umr2022/ ",
            //        StartAt = new DateTime(2021, 12, 20, 11, 24, 00),
                    
            //    }
            //};

            //var categories = new List<Category>()
            //{
            //    new Category()
            //    {
            //        //CategoryId = 1,
            //        CategoryName = "Universal"
            //    },
            //    new Category()
            //    {
            //        //CategoryId = 2,
            //        CategoryName = "Quarantine",
            //    }
            //};

            // And the one in DAL => DbInitializer folder
            //string[] roles = new string[] { "Admin", "Manager" };

            //foreach (string role in roles)
            //{
            //    var roleStore = new RoleStore<IdentityRole>(context);

            //    if (!context.Roles.Any(r => r.Name == role))
            //    {
            //        roleStore.CreateAsync(new IdentityRole(role));
            //    }
            //}

            //var eventCategories = new List<EventCategory>()
            //{
            //    new EventCategory
            //    {
            //        //EventCategoryId = 1,
            //        CategoryId = 1,
            //        EventId = 1
            //    },
            //    new EventCategory
            //    {
            //        //EventCategoryId = 2,
            //        CategoryId = 2,
            //        EventId = 1
            //    },
            //    new EventCategory
            //    {
            //        //EventCategoryId = 3,
            //        CategoryId = 1,
            //        EventId = 2
            //    },
            //};

            
            //var subscriptions = new List<Subscription>()
            //{
            //    new Subscription
            //    {
            //        SubscriptionId = 1,
            //    },
            //    new Subscription
            //    {
            //        SubscriptionId = 2,
            //    }
            //};

            //var subscriptionEvents = new List<SubscriptionEvent>()
            //{
            //    new SubscriptionEvent
            //    {
            //        //SubscriptionEventId = 1,
            //        EventId = 1,
            //        SubscriptionId = 1
            //    },
            //    new SubscriptionEvent
            //    {
            //        //SubscriptionEventId = 2,
            //        EventId = 2,
            //        SubscriptionId = 1
            //    },
            //    new SubscriptionEvent
            //    {
            //        //SubscriptionEventId = 3,
            //        EventId = 2,
            //        SubscriptionId = 2
            //    },
            //};

            //var notificationTypes = new List<NotificationType>()
            //{
            //    new NotificationType
            //    {
            //        //NotificationTypeId = 1,
            //        NotificationName = "Telegram"
            //    },
            //    new NotificationType
            //    {
            //        //NotificationTypeId = 2,
            //        NotificationName = "Instagram"
            //    },
            //    new NotificationType
            //    {
            //        //NotificationTypeId = 3,
            //        NotificationName = "Discord",
            //    }
            //};

            //var notificationTypeSubscriptions = new List<NotificationTypeSubscription>()
            //{
            //    new NotificationTypeSubscription
            //    {
            //        //NotificaitonTypeSubscriptionId = 1,
            //        //NotificationTypeId = 1,
            //        TelegramKey = "@Nicolas_Cage525",
            //        SubscriptionId = 1,
            //    },
            //    new NotificationTypeSubscription
            //    {
            //        //NotificaitonTypeSubscriptionId = 2,
            //        //NotificationTypeId = 2,
            //        InstagramKey = "@DenVozniuk007",
            //        SubscriptionId = 2,
            //    }
            //};

            //UserManager<ApplicationUser> userManager = services.GetService<UserManager<ApplicationUser>>();

            //if (userManager.FindByEmailAsync("mykola.kalinichenko@oa.edu.ua").Result == null)
            //{
            //    // https://stackoverflow.com/questions/50785009/how-to-seed-an-admin-user-in-ef-core-2-1-0

            //    ApplicationUser user = new ApplicationUser
            //    {
            //        UserName = "Kolya",
            //        Email = "mykola.kalinichenko@oa.edu.ua"
            //    };

            //    IdentityResult result = userManager.CreateAsync(user, "Kolka1337").Result;

            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRoleAsync(user, "Admin").Wait();
            //    }
            //    else
            //    {
            //        logger.LogInformation("User seeding failed!");
            //    }
            //}

            //string[] roles = new string[] { "Administrator", "Editor" };

            //foreach (string role in roles)
            //{
            //    var roleStore = new RoleStore<IdentityRole>(context);

            //    if (!context.Roles.Any(r => r.Name == role))
            //    {
            //        roleStore.CreateAsync(new IdentityRole(role));
            //    }
            //}

            //context.Events.AddRangeAsync(events);

            //context.Categories.AddRangeAsync(categories);

            //context.Subscriptions.AddRange(subscriptions);
            
            //context.EventCategories.AddRange(eventCategories);

            //context.NotificationTypes.AddRange(notificationTypes);

            //context.NotificationTypeSubscription.AddRange(notificationTypeSubscriptions);

            //context.SubscriptionEvents.AddRange(subscriptionEvents);

            //AssignRoles(services, user.Email, roles);

            context.SaveChangesAsync();

            logger.LogInformation("Finished seeding the database.");
        }
    }
}