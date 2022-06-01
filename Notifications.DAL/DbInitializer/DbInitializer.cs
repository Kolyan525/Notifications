using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
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
        readonly IMapper mapper;

        public DbInitializer(NotificationsContext context, IServiceProvider services, ILogger<DbInitializer> logger, IMapper mapper)
        {
            this.context = context;
            this.services = services;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task Initialize()
        {
            await SeedCategories();
            await SeedEvents();
            await SeedNotificationTypes();
            await SeedUsersAndRoles(services);

            logger.LogInformation("Finished seeding the database.");
            await context.SaveChangesAsync();
        }

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
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Lecture"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Festivals-fairs"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Master-Class"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Organizational"
            });

            logger.LogInformation("Starting to seed Categories");

            await context.SaveChangesAsync();
        }

        public async Task SeedEvents()
        {
            var categoryUniversal = context.Categories.First(x => x.CategoryName == "Universal");
            var categoryQuarantine = context.Categories.First(x => x.CategoryName == "Quarantine");
            var categoryLecture = context.Categories.First(x => x.CategoryName == "Lecture");
            var categoryFestivalsFairs = context.Categories.First(x => x.CategoryName == "Festivals-fairs");
            var categoryMasterClass = context.Categories.First(x => x.CategoryName == "Master-Class");
            var categoryOrganizational = context.Categories.First(x => x.CategoryName == "Organizational");

            await CreateEventIfNotExists(new Event
            {
                Title = "Online Learning in NaU\"OA\" Starts",
                Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore an online learning will be established.",
                ShortDesc = "Very short description for online learning",
                EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA",
                StartAt = new DateTime(2021, 11, 20, 9, 00, 00).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    },
                    new EventCategory
                    {
                        Category = categoryQuarantine
                    },
                    new EventCategory
                    {
                        Category = categoryOrganizational
                    }
                }
            });

            await CreateEventIfNotExists(new Event
            {
                Title = "International rating",
                Description = "Congratulations, My name is Natalia, I deal with international rankings and NaU\"OA\" membership in them. This year, U - Multirank is conducting a survey among students majoring in Computer Science. Please contribute to the high place of NaU\"OA\" in this ranking by filling out a small survey. I pinned the letter below",
                ShortDesc = "Very short description for international rating",
                EventLink = "https://che-survey.de/uc/umr2022/",
                StartAt = new DateTime(2021, 12, 20, 11, 24, 00).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    }
                }
            });

            await CreateEventIfNotExists(new Event
            {
                Title = "Discussion of the Regulations",
                Description = "Public discussion of the Regulations preject on mentoring in NaU\"OA\"",
                ShortDesc = "We should discuss that, really!",
                EventLink = "https://che-survey.de/uc/umr2022/",
                StartAt = new DateTime(2022, 2, 10, 12, 00, 00).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    },
                    new EventCategory
                    {
                        Category = categoryOrganizational
                    }
                }
            });

            await CreateEventIfNotExists(new Event
            {
                Title = "Lecture by Rustem Ablyatif",
                Description = "Open University. Lecture by Rustem Ablyatif. Lessons for Ukraine through the prism of the history and modernity of Turkey Republic",
                ShortDesc = "You should come and listen to our lection. It tackles some important information. Speaker is Rustem Ablyatif.",
                EventLink = String.Empty,
                StartAt = new DateTime(2022, 2, 10, 15, 30, 00).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    },
                    new EventCategory
                    {
                        Category = categoryLecture
                    }
                }
            });

            await CreateEventIfNotExists(new Event
            {
                Title = "Master class about labor law",
                Description = "You will find: - 40 minutes of practical information; -20 minutes Q &Asession: answers to all questions; -5 innovations in labor legislation that every educator should know: about wages during quarantine, unpaid leave, remote and homework; -real success cases of protection of labor rights by educators.",
                ShortDesc = "Online master class \"TOP - 5 short stories in labor law for educators\"",
                EventLink = "https://forms.gle/AUCJ8w4Tjeb74Lpw8",
                StartAt = new DateTime(2022, 2, 10, 15, 30, 00).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
                    },
                    new EventCategory
                    {
                        Category = categoryLecture
                    },
                    new EventCategory
                    {
                        Category = categoryMasterClass
                    }
                }
            });

            logger.LogInformation("Starting to seed Events");

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

        public async Task SeedNotificationTypes()
        {
            await CreateNotificationTypeIfNotExists("Telegram");
            await CreateNotificationTypeIfNotExists("Discord");
            await CreateNotificationTypeIfNotExists("Instagram");

            logger.LogInformation("Starting to seed NotificationTypes");
            await context.SaveChangesAsync();
        }

        public async Task SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityResult roleResult;
            string[] roles = new string[] { "Admin", "Manager" };

            foreach (var roleName in roles)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    logger.LogInformation("Starting to seed roles");
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string a1 = "denys.vozniuk@oa.edu.ua";
            string a2 = "mykola.kalinichenko@oa.edu.ua";
            string a3 = "oleksandra.kravets@oa.edu.ua";

            await CreateUserIfNotExists(a1);
            await CreateUserIfNotExists(a2);
            await CreateUserIfNotExists(a3);

            //UserManager<ApplicationUser> userManager = services.GetService<UserManager<ApplicationUser>>();

            //ApplicationUser admin = new ApplicationUser
            //{
            //    FirstName = "Kolya",
            //    LastName = "Kalina",
            //    Email = "mykola.kalinichenko@oa.edu.ua",
            //    EmailConfirmed = true
            //};

            //ApplicationUser manager = new ApplicationUser
            //{
            //    FirstName = "Denys",
            //    LastName = "Vozniuk",
            //    Email = "denys.vozniuk@oa.edu.ua",
            //    EmailConfirmed = true
            //};
            
            //ApplicationUser admin2 = new ApplicationUser
            //{
            //    FirstName = "Sasha",
            //    LastName = "Kravec",
            //    Email = "oleksandra.kravets@oa.edu.ua",
            //    EmailConfirmed = true
            //};

            //admin.UserName = admin.Email;
            //admin2.UserName = admin2.Email;
            //manager.UserName = manager.Email;

            //var adminEmailExists = userManager.FindByEmailAsync(admin.Email).Result == null;
            //var admin2EmailExists = userManager.FindByNameAsync(admin2.Email).Result == null;
            //var admin2UsernameExists = userManager.FindByNameAsync(admin2.Email).Result == null;

            //logger.LogInformation("Starting to seed Admins");
            //if (!adminEmailExists)
            //{
            //    IdentityResult adminResult = userManager.CreateAsync(admin, "Kolk@1337").Result;
            //    IdentityResult adminResult2 = userManager.CreateAsync(admin2, "Sash@1234").Result;

            //    if (adminResult.Succeeded && adminResult2.Succeeded)
            //    {
            //        userManager.AddToRoleAsync(admin, "Admin").Wait();
            //        userManager.AddToRoleAsync(admin2, "Admin").Wait();
            //    }
            //    else
            //        logger.LogInformation("Admin seeding failed!");
            //}

            //var managerEmailExists = userManager.FindByEmailAsync(manager.Email).Result == null;
            //var managerUsernameExists = userManager.FindByNameAsync(manager.UserName).Result == null;

            //if (managerEmailExists && managerUsernameExists)
            //{
            //    logger.LogInformation("Starting to seed Manager");
            //    // https://stackoverflow.com/questions/50785009/how-to-seed-an-admin-user-in-ef-core-2-1-0

            //    IdentityResult managerResult = userManager.CreateAsync(manager, "Deny@1337").Result;

            //    if (managerResult.Succeeded)
            //        userManager.AddToRoleAsync(manager, "Manager").Wait();
            //    else
            //        logger.LogInformation("Manager seeding failed!");
            //}
        }

        public async Task CreateEventIfNotExists(Event vent)
        {
            var @event = await context.Events.FirstOrDefaultAsync(
                e => e.Title == vent.Title &&
                e.Description == vent.Description &&
                e.ShortDesc == vent.ShortDesc &&
                e.EventLink == vent.EventLink &&
                e.StartAt == vent.StartAt);
            if (@event == null)
                await context.Events.AddAsync(vent);
            //else
            //{
            //    // possible, we need to have a logic of how to update an existing event

            //    @event.Title = vent.Title;
            //    @event.Description = vent.Description;
            //    @event.ShortDesc = vent.ShortDesc;
            //    @event.EventLink = vent.EventLink;
            //    @event.StartAt = vent.StartAt;
            //    @event.EventCategories = vent.EventCategories;

            //    //mapper.Map(vent, @event);
            //    context.Attach(@event);
            //    context.Entry(@event).State = EntityState.Modified;
            //}
        }

        public async Task CreateCategoryIfNotExists(Category category)
        {
            var cat = await context.Categories.FirstOrDefaultAsync(e => e.CategoryName == category.CategoryName);
            if (cat == null)
            {
                await context.Categories.AddAsync(category);
            }
        }

        public async Task CreateNotificationTypeIfNotExists(string notificationTypeName)
        {
            var nt = await context.NotificationTypes.FirstOrDefaultAsync(n => n.NotificationName == notificationTypeName);
            if (nt == null)
            {
                var newNotificationType = new NotificationType
                {
                    NotificationName = notificationTypeName
                };
                await context.NotificationTypes.AddAsync(newNotificationType);
            }
        }

        public async Task CreateUserIfNotExists(string email)
        {
            UserManager<ApplicationUser> userManager = services.GetService<UserManager<ApplicationUser>>();

            var userExists = await userManager.FindByEmailAsync(email) != null;

            if (!userExists)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                };

                IdentityResult userResult = await userManager.CreateAsync(user);

                if (userResult.Succeeded)
                    logger.LogInformation($"User {user.Email} created");
                else
                    logger.LogInformation($"User {user.Email} seeding failed!");
            }
            else
                logger.LogInformation($"User {email} already exists");
        }
    }
}