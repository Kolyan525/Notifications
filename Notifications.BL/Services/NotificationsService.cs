﻿using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.BL.IRepository;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Notifications.BL.Services
{
    public class NotificationsService
    {
        readonly IUnitOfWork unitOfWork;
        private readonly TelegramBotClient botClient;
        readonly IMapper mapper;

        public NotificationsService(IUnitOfWork unitOfWork, TelegramBot bot)
        {
            this.unitOfWork = unitOfWork;
            this.botClient = bot.GetBot().Result;
        }

        public async Task<IApiResponse> GetEventCategories(long id)
        {
            if (id > 0)
            {
                var vent = await unitOfWork.Events.GetFirstOrDefault(
                    x => x.EventId == id,
                    include: x => x
                        .Include(x => x.EventCategories)
                        .ThenInclude(x => x.Category)
                );

                var categories = new List<string>();
                foreach (var item in vent.EventCategories)
                {
                    categories.Add(item.Category.CategoryName);
                }

                return ApiResponse.Ok(categories);
            }

            return ApiResponse.BadRequest("Submitted data was invalid");
        }

        /*
        public async Task<IApiResponse<ICollection<Event>>> GetSubscribedEvents(string UserTelegramKey = null, string UserInstagramKey = null, string UserDiscordKey = null)
        {
            try
            {
                NotificationTypeSubscription nts;
                if (UserTelegramKey != null)
                {
                    nts = await unitOfWork.NotificationTypeSubscriptions.GetFirstOrDefault(
                        x => x.TelegramKey.Equals(UserTelegramKey),
                        include: x => x
                            .Include(nts => nts.Subscription)
                            .ThenInclude(s => s.SubscriptionEvents)
                            .ThenInclude(se => se.Event)
                    );
                }
                else if (UserInstagramKey != null)
                {
                    nts = await unitOfWork.NotificationTypeSubscriptions.GetFirstOrDefault(
                        x => x.InstagramKey.Equals(UserInstagramKey),
                        include: x => x
                            .Include(nts => nts.Subscription)
                            .ThenInclude(s => s.SubscriptionEvents)
                            .ThenInclude(se => se.Event)
                    );
                }
                else
                {
                    nts = await unitOfWork.NotificationTypeSubscriptions.GetFirstOrDefault(
                        x => x.DiscordKey.Equals(UserDiscordKey),
                        include: x => x
                            .Include(nts => nts.Subscription)
                            .ThenInclude(s => s.SubscriptionEvents)
                            .ThenInclude(se => se.Event)
                    );
                }

                var events = new List<Event>();
                foreach (var item in nts.Subscription.SubscriptionEvents)
                {
                    events.Add(item.Event);
                }

                logger.LogInformation($"Successfully executed {nameof(GetSubscribedEvents)}");
                return events;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetSubscribedEvents)}");
                return null;
            }
        }
        */

        // Seems to work 9.5.22
        public async Task<IApiResponse> AddCategoryToEvent(long eventId, long categoryId)
        {
            var cat = await unitOfWork.Categories.Get(x => x.CategoryId == categoryId);
            if (cat == null) return ApiResponse.NotFound("Category was not found");

            var @event = await unitOfWork.Events.Get(x => x.EventId == eventId, new List<string> { "EventCategories" });
            if (@event == null) return ApiResponse.NotFound("Event was not found");

            // Check if EC already exists within event
            var ch = @event.EventCategories.FirstOrDefault(x => x.CategoryId == cat.CategoryId && x.Event.Title == @event.Title);
            if (ch != null)
                return ApiResponse.Conflict("This category is already assigned to the event");

            @event.EventCategories.Add(new EventCategory { CategoryId = cat.CategoryId });

            unitOfWork.Events.Update(@event);
            await unitOfWork.Save();

            return ApiResponse.Ok(@event);
        }

        public async Task<IApiResponse> RemoveCategoryFromEvent(long eventId, long categoryId)
        {
            var cat = await unitOfWork.Categories.Get(x => x.CategoryId == categoryId);
            if (cat == null) return ApiResponse.NotFound("Category was not found");

            var @event = await unitOfWork.Events.Get(x => x.EventId == eventId, new List<string> { "EventCategories" });
            if (@event == null) return ApiResponse.NotFound("Event was not found");

            // Check if EC already exists within event
            //var ch = @event.EventCategories.FirstOrDefault(x => x.CategoryId == cat.CategoryId && x.Event.Title == @event.Title);
            //if (ch == null)
            //    return ApiResponse.Conflict("This category haven`t been assigned to the event");

            var ec = await unitOfWork.EventCategories.GetFirstOrDefault(x => x.EventId == eventId, x => x.CategoryId == categoryId);

            if (ec == null)
            {
                return ApiResponse.NotFound("Event category was not found");
            }

            await unitOfWork.EventCategories.Delete(ec.EventCategoryId);
            @event.EventCategories.Remove(ec);
            //unitOfWork.Events.Update(@event);
            await unitOfWork.Save();

            return ApiResponse.Ok(@event);
        }

        public async Task<IApiResponse> AddCategoriesToEvent(long eventId, List<long> categories)
        {
            if (eventId > 0 || categories != null)
            {
                var @event = await unitOfWork.Events.GetFirstOrDefault(
                    e => e.EventId == eventId,
                    include: e => e
                        .Include(e => e.EventCategories)
                            .ThenInclude(ec => ec.Category));

                foreach (var item in categories)
                {
                    await AddCategoryToEvent(@event.EventId, item);
                }

                return ApiResponse.Ok(@event);
            }
            else return ApiResponse.BadRequest();
        }

        public async Task<IApiResponse> RemoveCategoriesFromEvent(long eventId, List<long> categories)
        {
            if (eventId > 0 || categories != null)
            {
                var @event = await unitOfWork.Events.GetFirstOrDefault(
                    e => e.EventId == eventId,
                    include: e => e
                        .Include(e => e.EventCategories)
                            .ThenInclude(ec => ec.Category));

                foreach (var item in categories)
                {
                    await RemoveCategoryFromEvent(@event.EventId, item);
                }

                return ApiResponse.Ok(@event);
            }
            else return ApiResponse.BadRequest();
        }

        public async Task<IApiResponse> UpdateEventCategories(long eventId, List<long> categories, EventWithCategoriesDTO eventDTO)
        {
            if (eventId > 0 || categories != null)
            {
                var @event = await unitOfWork.Events.GetFirstOrDefault(
                    e => e.EventId == eventId,
                    include: e => e
                        .Include(e => e.EventCategories)
                            .ThenInclude(ec => ec.Category));

                @event.Title = @eventDTO.Title;
                @event.Description = @eventDTO.Description;
                @event.ShortDesc = @eventDTO.ShortDesc;
                @event.StartAt = @eventDTO.StartAt;
                @event.EventLink = @eventDTO.EventLink;
                @event.Location = @eventDTO.Location;
                @event.Price = eventDTO.Price;

                var categs = new List<long>();
                foreach (var item in @event.EventCategories)
                {
                    categs.Add(item.CategoryId);
                }

                await RemoveCategoriesFromEvent(@event.EventId, categs);

                //unitOfWork.Events.Update(@event);
                //var result = await AddCategoriesToEvent(@event.EventId, categories);

                foreach (var category in categories)
                {
                    @event.EventCategories.Add(new EventCategory { CategoryId = category });
                }

                unitOfWork.Events.Update(@event);

                await unitOfWork.Save();

                return ApiResponse.Ok(@event);
            }
            else return ApiResponse.BadRequest();
        }

        /// <summary>
        /// Subscribes to an event using telegram id and user id
        /// </summary>
        /// <param name="eventId">Event ID parameter, long</param>
        /// <param name="userId">Should be Telegram ID, string</param>
        /// <returns></returns>
        public async Task<IApiResponse> SubscribeToEvent(long eventId, string userId)
        {
            var exists = SubscriptionExists(eventId, userId);
            if (exists.Result) return ApiResponse.Conflict("Event subscription already exists");

            var notificationTelegram = await unitOfWork.NotificationTypes.Get(x => x.NotificationName == "Telegram");
            
            var subEvent = new SubscriptionEvent
            {
                EventId = eventId,
                Subscription = new Subscription
                {
                    NotificationTypeSubscriptions = new List<NotificationTypeSubscription>
                    {
                        new NotificationTypeSubscription
                        {
                            TelegramKey = userId,
                            NotificationTypeId = notificationTelegram.NotificationTypeId
                        }
                    }
                }
            };

            await unitOfWork.SubscriptionEvents.Insert(subEvent);
            await unitOfWork.Save();

            var sub = subEvent.Subscription;
            sub.SubscriptionEventId = subEvent.SubscriptionEventId;

            var ntsub = await unitOfWork.NotificationTypeSubscriptions.Get(x => x.SubscriptionId == subEvent.SubscriptionId);
            sub.NotificationTypeSubscriptionId = ntsub.NotificaitonTypeSubscriptionId;

            await unitOfWork.Save();

            return ApiResponse.Ok("Succesfully subscribed!");
        }

        public async Task<IApiResponse> SubscribeToCategory(long categoryId, string userId)
        {
            if (!unitOfWork.Categories.Exists(categoryId)) return ApiResponse.NotFound("Category was not found");
            if (string.IsNullOrEmpty(userId)) return ApiResponse.BadRequest("User ID is empty");    // Should probably add validation

            var notificationTelegram = await unitOfWork.NotificationTypes.Get(x => x.NotificationName == "Telegram");

            var eventCategories = await unitOfWork.EventCategories.GetAll(ec => ec.CategoryId == categoryId);

            foreach (var eventCategory in eventCategories)
                await SubscribeToEvent(eventCategory.EventId, userId);

            //await unitOfWork.Save();
            
            return ApiResponse.Ok("Succesfully subscribed to the category!");
        }

        public async Task<IApiResponse> UnsubscribeFromEvent(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId) && string.IsNullOrEmpty(userId)) { return ApiResponse.BadRequest("Submitted data was invalid"); }

            var ntsubs = await unitOfWork.NotificationTypeSubscriptions.GetAll(x => x.TelegramKey == userId);
            var subEvents = await unitOfWork.SubscriptionEvents.GetAll(x => x.EventId == eventId);

            if (ntsubs == null) return ApiResponse.NotFound("No NTS was found");
            if (subEvents == null) return ApiResponse.NotFound("No SubscriptionEvents was found");

            foreach (var ntsub in ntsubs)
            {
                foreach (var subEvent in subEvents)
                {
                    if (subEvent.SubscriptionId == ntsub.SubscriptionId)
                    {
                        await unitOfWork.Subscriptions.Delete(subEvent.SubscriptionId);

                        //await unitOfWork.Save();

                        await unitOfWork.NotificationTypeSubscriptions.Delete(ntsub.NotificaitonTypeSubscriptionId);

                        await unitOfWork.SubscriptionEvents.Delete(subEvent.SubscriptionEventId);

                        await unitOfWork.Save();

                        return ApiResponse.NoContent("Successfully unsubscribed from event");
                    }
                }
            }

            // if not above, then i haven't subbed or nts/subEvent doesn't exist
            return ApiResponse.Conflict("Something went wrong. Probably, you aren't subscribed");
        }

        // Seems to work
        public async Task<IApiResponse> UnsubscribeFromCategory(long categoryId, string userId)
        {
            if (!unitOfWork.Events.Exists(categoryId) && string.IsNullOrEmpty(userId)) { return ApiResponse.BadRequest("Submitted data was invalid"); }

            var ntsubs = await unitOfWork.NotificationTypeSubscriptions.GetAll(x => x.TelegramKey == userId);

            if (ntsubs == null) return ApiResponse.NotFound("No NTS was found");

            var eventsWithCategory = await unitOfWork.NotificationTypeSubscriptions.GetAllHere(nts => nts.TelegramKey == userId,
                include: nts => nts
                    .Include(nts => nts.Subscription)
                    .ThenInclude(s => s.SubscriptionEvents)
                    .ThenInclude(se => se.Event)
                    .ThenInclude(e => e.EventCategories.Where(ec => ec.CategoryId == categoryId))
            );

            foreach (var nts in eventsWithCategory)
            {
                foreach (var sub in nts.Subscription.SubscriptionEvents)
                {
                    foreach (var ec in sub.Event.EventCategories)
                    {
                        //Console.WriteLine($"{ec.Event.Title}, {ec.CategoryId}");
                        await UnsubscribeFromEvent(ec.EventId, userId);
                    }
                }
            }
            
            return ApiResponse.NoContent("Successfully unsubscribed from category");

            // if not above, then i haven't subbed or nts/subEvent doesn't exist
            //return ApiResponse.Conflict("Something went wrong. Probably, you aren't subscribed");
        }

        public async Task<IApiResponse> ListOfSubscribedEvents(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ApiResponse.BadRequest("Submitted data was invalid. Please try again!");

            var subbed = await unitOfWork.NotificationTypeSubscriptions.GetAllHere(
                nts => nts.TelegramKey == userId,
                include: nts => nts
                    .Include(nts => nts.Subscription)
                    .ThenInclude(s => s.SubscriptionEvents)
                    .ThenInclude(se => se.Event));

            if (subbed != null)
            {
                var vents = new List<Event>();
                foreach (var b in subbed)
                {
                    foreach (var se in b.Subscription.SubscriptionEvents)
                    {
                        vents.Add(se.Event);
                    }
                }
                return ApiResponse.Ok(vents);
            }

            return ApiResponse.NotFound($"No subscriptions was found with following ID: \"{userId}\"");
        }


        // How to validate seacrh from spaces, empty strings etc.?
        // Search using specific date format
        public async Task<IApiResponse<IList<Event>>> SearchEvents(string search)
        {
            IList<Event> events = null;
            if (string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search)) return null;
            
            var date = DateTime.TryParseExact(search, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (!date)
            {
                //events = await unitOfWork.Events.GetAll(
                //        x => x.Title.Contains(search) ||
                //        x.Description.Contains(search) ||
                //        x.ShortDesc.Contains(search));

                var IsCategory = await unitOfWork.Categories.Get(x => x.CategoryName == search);

                if (IsCategory == null)
                {
                    events = await unitOfWork.Events.GetAll(
                        x => x.Title.Contains(search) ||
                        x.Description.Contains(search) ||
                        x.ShortDesc.Contains(search));
                }
                else
                {
                    events = await unitOfWork.Events.GetAllHere(
                            x => x.Title.Contains(search) ||
                            x.Description.Contains(search) ||
                            x.ShortDesc.Contains(search),
                        include: x => x.
                            Include(e => e.EventCategories.Where(e => e.Category == IsCategory))
                                .ThenInclude(ec => ec.Category));
                }
                //x.StartAt.Date == Convert.ToDateTime(search))
                //x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null))
            }
            else if (date)
            {
                events = await unitOfWork.Events.GetAll(
                    x => x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null));
            }

            return ApiResponse.Ok(events);
        }

        public async Task<IApiResponse> FilterEvents(List<string> query)
        {
            if (query == null || query.Count == 0) return ApiResponse.BadRequest("The query was empty");

            var categories = await unitOfWork.Categories.GetAll(x => query.Contains(x.CategoryName));

            var events = await unitOfWork.Events.GetAllHere(
                include: x => x
                    .Include(ec => ec.EventCategories)
                    .ThenInclude(ec => ec.Category));

            List<Event> filtered = new List<Event>();

            foreach (var @event in events)
            {
                if (EventWithCategories(@event, categories.ToList()))
                {
                    filtered.Add(@event);
                }
            }
            return ApiResponse.Ok(filtered);
        }

        public bool EventWithCategories(Event @event, List<Category> categories)
        {
            int checks = 0;
            foreach (var category in categories)
            {
                foreach (var ec in @event.EventCategories)
                {
                    if(ec.Category.CategoryName.Equals(category.CategoryName))
                        checks++;
                }
            }
            if (checks == categories.Count)
                return true;

            return false;
        }

        // Once a: week -+ 60m, day +- 5m, hour +- 2m.
        // If event falls within the time frame then we should execute notification for all users just once (time frame can be true multiple times)
        /// <summary>
        /// Check if the event is going to be held within following time span
        /// </summary>
        /// <param name="event">The Event model class</param>
        /// <param name="timeSpan">The time, before which the user gets notified</param>
        /// <param name="interval">The checking interval</param>
        /// <returns></returns>
        public bool IsDue(Event @event, TimeSpan timeSpan, TimeSpan interval, DateTime today)
        {
            // 32, 10 | example
            // Event will be in 40 m | example
            var timeBeforeEvent = @event.StartAt.ToLocalTime().Subtract(today.ToLocalTime());

            bool due = timeBeforeEvent <= timeSpan.Add(interval) && timeBeforeEvent >= timeSpan.Subtract(interval) ? true : false;
            //DateTime combined = date.Add(timeSpan);
            
            Console.WriteLine("Now ({0}) + time ({1}) = {2}" +
                "\nEvent date {3} ({4})", today, timeSpan, today.Add(timeSpan), @event.StartAt.ToLocalTime(), due);

            return due;
        }

        public async Task<IApiResponse<List<string>>> GetEventSubscribedUsersId(long eventId)
        {
            var subEvents = await unitOfWork.SubscriptionEvents.GetAllHere(x => x.Event.EventId == eventId, 
                include: x => x.Include(se => se.Subscription)
                               .ThenInclude(s => s.NotificationTypeSubscriptions));

            var users = new List<string>();

            foreach (var se in subEvents)
            {
                foreach (var nts in se.Subscription.NotificationTypeSubscriptions)
                {
                    users.Add(nts.TelegramKey);
                }
            }

            return ApiResponse.Ok(users);
        }

        public async Task<IApiResponse> GetOrderedByDateEvents()
        {
            var events = await unitOfWork.Events.GetAll(orderBy: e => e.OrderBy(e => e.StartAt));

            var result = events == null ? ApiResponse.NotFound() : ApiResponse.Ok(events);

            return result;
        }

        public async Task CheckEvents(TimeSpan timeSpan, TimeSpan interval)
        {
            var events = await unitOfWork.Events.GetAll();

            var today = DateTime.UtcNow.ToLocalTime();
            Console.WriteLine($"Recurring local time {today}");

            foreach (var @event in events)
            {
                var users = await GetEventSubscribedUsersId(@event.EventId);
                if(IsDue(@event, timeSpan, interval, today))
                {
                    Console.WriteLine($"The event \"{@event.Title}\" will take place in {timeSpan} time span!");
                    foreach (var user in users.Data)
                    {
                        await NotifyUser(@event, user);
                    }
                }
            }
        }

        public async Task ScheduleNotification(Event @event, string userId)
        {
            var timeToEvent = @event.StartAt.ToLocalTime().Subtract(TimeSpan.FromMinutes(1));

            BackgroundJob.Schedule(
                () => NotifyUser(@event, userId),
                timeToEvent);
        }

        public async Task NotifyUser(Event @event, string userName)
        {
            // Send message to user
            var message = $"<b>Скоро відбудеться подія на яку ви підписалися!</b>\n\n" +
                $"<u><b>{@event.Title}</b></u>\n\n<b>Опис події:</b> {@event.Description}." +
                $"\n\n<b>Початок: {@event.StartAt}</b>";
            if (await SubscriptionExists(@event.EventId, userName))
            {
                await botClient.SendTextMessageAsync(userName, message, parseMode: ParseMode.Html);
                Console.WriteLine($"{userName}, has upcoming events - {@event.Title}|{@event.StartAt.ToUniversalTime()}");
            }
        }

        public async Task<bool> SubscriptionExists(long eventId, string userId)
        {
            if (eventId <= 0 && userId == String.Empty)
            {
                return false;
            }

            var ntsubs = await unitOfWork.NotificationTypeSubscriptions.GetAll(x => x.TelegramKey == userId);
            var subscriptionEvents = await unitOfWork.SubscriptionEvents.GetAll(s => s.EventId == eventId);

            foreach (var nt in ntsubs)
            {
                foreach (var sub in subscriptionEvents)
                {
                    if (nt.SubscriptionId == sub.SubscriptionId) return true;
                }
            }
            return false;
        }
    }
}

/* 
 * // TODO:
 * filter Events based on Categories
 * Sort Events
 * 
    передбачається, що наприклад категорії подій будуть додаватися спершу, тоді вже самі події
    коли користувач підписуватиметься, ви спершу створите його підписку, а тоді додасте підписку до якоїсь категорії подій



    Done: search event, subscribe to event, unsubscribe to event, get list of subbed events, add category to event
          subscribe to category,
 */

