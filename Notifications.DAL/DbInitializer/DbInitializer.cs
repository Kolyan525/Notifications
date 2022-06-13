﻿using Microsoft.AspNetCore.Identity;
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
                CategoryName = "Загальноакадемічні"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Карантин"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Лекції"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Фестивалі"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Майстер-класи"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Організаційні"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Розваги"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Навчання"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Всі факультети"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Економіка"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Право"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "Міжнародні відносини"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "РГМ"
            });
            await CreateCategoryIfNotExists(new Category
            {
                CategoryName = "ГУМ"
            });

            logger.LogInformation("Starting to seed Categories");

            await context.SaveChangesAsync();
        }

        public async Task SeedEvents()
        {
            var categoryUniversal = context.Categories.First(x => x.CategoryName == "Загальноакадемічні");
            var categoryQuarantine = context.Categories.First(x => x.CategoryName == "Карантин");
            var categoryLecture = context.Categories.First(x => x.CategoryName == "Лекції");
            var categoryFestivalsFairs = context.Categories.First(x => x.CategoryName == "Фестивалі");
            var categoryMasterClass = context.Categories.First(x => x.CategoryName == "Майстер-класи");
            var categoryOrganizational = context.Categories.First(x => x.CategoryName == "Організаційні");

            await CreateEventIfNotExists(new Event
            {
                Title = "Перехід на онлайн навчання",
                Description = "Шановні студенти! На наступні три тижні нам потрібно всім разом (студентам і викладачам) об’єднатися, щоб не втратити дорогоцінний час другого семестру. Тому навчанння продовжиться в онлайн режимі.",
                ShortDesc = "Шановні студенти! На наступні три тижні нам потрібно всім разом...",
                EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA",
                StartAt = DateTime.Now.AddDays(5).ToUniversalTime(),
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
                Title = "Міжнародний рейтинг",
                Description = "Вітаю, мене звати Наталія, я займаюся міжнародними рейтингами та членством в них НаУ\"ОА\". Цього року U - Multirank проводить опитування серед студентів спеціальності «Комп’ютерні науки». Будь ласка, внесіть свій внесок у високе місце NaU \"OA\" у цьому рейтингу, заповнивши невелике опитування. Я закріпила лист нижче",
                ShortDesc = "Вітаю, мене звати Наталія, я займаюся міжнародними рейтингами та членством в них...",
                EventLink = "https://che-survey.de/uc/umr2022/",
                StartAt = DateTime.Now.AddDays(7).ToUniversalTime(),
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
                Title = "Обговорення проекту",
                Description = "Публічне обговорення проекту \"Положення про наставництво\" в НаУ \"ОА\"",
                ShortDesc = "Публічне обговорення проекту...",
                EventLink = "https://che-survey.de/uc/umr2022/",
                StartAt = DateTime.Now.AddDays(9).ToUniversalTime(),
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
                Title = "Лекція Рустема Аблятіфа",
                Description = "Відкритий Університет. Лекція Рустема Аблятіфа - «Уроки для України крізь призму історії та сучасності Турецької Республіки»",
                ShortDesc = "Ви повинні прийти і послухати нашу лекцію. Це стосується нашої держави. Спікер – Рустем Аблятіф...",
                EventLink = string.Empty,
                StartAt = DateTime.Now.AddDays(11).ToUniversalTime(),
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
                Title = "Майстер-клас з трудового права",
                Description = "Ви знайдете: - 40 хвилин практичної інформації; -20 хвилин Q&A: відповіді на всі запитання; -5 нововведень у трудовому законодавстві, які повинен знати кожен вихователь: про заробітну плату під час карантину, неоплачувану відпустку, дистанційне та домашнє завдання; -реальні успішні випадки захисту трудових прав освітянами.",
                ShortDesc = "Онлайн майстер-клас «ТОП - 5 оповідань з трудового права для освітян»",
                EventLink = "https://forms.gle/AUCJ8w4Tjeb74Lpw8",
                StartAt = DateTime.Now.AddDays(13).ToUniversalTime(),
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

            await CreateEventIfNotExists(new Event
            {
                Title = "Волонтерський фронт: про гуманітарну допомогу внутрішньо переміщеним особам",
                Description = "Національний університет \"Острозька академія\" організував передачу гуманітарної допомоги внутрішньо переміщеним особам та цивільному населенню Острога, які опинилися у складних життєвих обставинах і зараз проживають у гуртожитках Острозької академії.",
                ShortDesc = "Національний університет \"Острозька академія\" організував...",
                EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA",
                StartAt = DateTime.Now.AddDays(15).ToUniversalTime(),
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
                Title = "Інформаційний фронт: про епоху постправди",
                Description = "Національний університет «Острозька академія» в рамках Фонду гуманітарної підтримки ВНЗ України реалізує безкоштовний навчальний курс «Спільнота підтримки університету», який є продовженням та розширенням проекту «Психологічна підтримка вчителів і репетиторів» запущено в березні цього року.",
                ShortDesc = "Національний університет «Острозька академія» в рамках Фонду гуманітарної підтримки ВНЗ України реалізує...",
                EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA",
                StartAt = DateTime.Now.AddDays(17).ToUniversalTime(),
                EventCategories = new List<EventCategory>
                {
                    new EventCategory
                    {
                        Category = categoryUniversal
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
            string a1p = "Denys@1234";

            string a2 = "mykola.kalinichenko@oa.edu.ua";
            string a2p = "Kolya@1234";

            string a3 = "oleksandra.kravets@oa.edu.ua";
            string a3p = "Sasha@1234";

            await CreateUserIfNotExists(a1, a1p);
            await CreateUserIfNotExists(a2, a2p);
            await CreateUserIfNotExists(a3, a3p);

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
                e => e.Title == vent.Title ||
                e.Description == vent.Description ||
                e.ShortDesc == vent.ShortDesc ||
                e.EventLink == vent.EventLink ||
                e.StartAt == vent.StartAt);
            if (@event == null)
                await context.Events.AddAsync(vent);
            else
            {
                vent.EventId = @event.EventId;  
                mapper.Map(vent, @event);
                context.Events.Attach(@event);
                context.Entry(@event).State = EntityState.Modified;
            }
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

        public async Task CreateUserIfNotExists(string email, string password)
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

                IdentityResult userResult = await userManager.CreateAsync(user, password);

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
