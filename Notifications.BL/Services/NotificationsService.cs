using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Notifications.BL.IRepository;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.BL.Services
{
    public class NotificationsService
    {
        readonly IUnitOfWork unitOfWork;
        readonly ILogger<NotificationsService> logger;
        readonly IMapper mapper;

        public NotificationsService(IUnitOfWork unitOfWork, ILogger<NotificationsService> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task<ICollection<string>> GetEventCategories(long id)
        {
            try
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

                return categories;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*
        public async Task<ActionResult<ICollection<Event>>> GetSubscribedEvents(string UserTelegramKey = null, string UserInstagramKey = null, string UserDiscordKey = null)
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

        public async Task<Event> AddCategoryToEvent(long eventId, long categoryId)
        {
            var cat = await unitOfWork.Categories.Get(x => x.CategoryId == categoryId);
            var @event = await unitOfWork.Events.Get(x => x.EventId == eventId, new List<string> { "EventCategories" });

            // Check if EC already exists within event
            var ch = @event.EventCategories.FirstOrDefault(x => x.CategoryId == cat.CategoryId && x.Event.Title == @event.Title);
            if (ch != null)
            {
                return null;
            }

            @event.EventCategories.Add(new EventCategory { Category = cat });
            return @event;
        }

        /// <summary>
        /// Subscribes to an event using telegram id and user id
        /// </summary>
        /// <param name="eventId">Event ID parameter, long</param>
        /// <param name="userId">Should be Telegram ID, string</param>
        /// <returns></returns>
        public async Task<Subscription> Subscribe(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId) && userId == "") { return null; }

            var nts = await NotificationTypeSubscriptionExists(userId);
            var subscriptionEvent = await SubscriptionEventExists(eventId);

            var @event = await unitOfWork.Events.Get(x => x.EventId == eventId);
            
            var subEvent = new SubscriptionEvent();
            var sub = new Subscription();
            var ntsub = new NotificationTypeSubscription();

            // so if nts & subscriptionEvent != null -> subscription exists (?)
            if (nts != null && subscriptionEvent != null)
            {
                if (nts.SubscriptionId == subscriptionEvent.SubscriptionId)
                    return null;
            }

            //if (nts != null && subscriptionEvent == null)
            //{
            //    subEvent = new SubscriptionEvent
            //    {
            //        EventId = @event.EventId,
            //        Subscription = new Subscription
            //        {
            //            NotificationTypeSubscriptions = new List<NotificationTypeSubscription> { }
            //        }
            //    };

            //    await unitOfWork.SubscriptionEvents.Insert(subEvent);
            //    await unitOfWork.Save();

            //    sub = subEvent.Subscription;
            //    sub.SubscriptionEventId = subEvent.SubscriptionEventId;
            //    sub.NotificationTypeSubscriptions.Add(nts);

            //    sub.NotificationTypeSubscriptionId = nts.NotificaitonTypeSubscriptionId;

            //    await unitOfWork.Save();
            //    return sub;
            //}

            var notificationTelegram = await unitOfWork.NotificationTypes.Get(x => x.NotificationName == "Telegram");
            
            subEvent = new SubscriptionEvent
            {
                EventId = @event.EventId,
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

            sub = subEvent.Subscription;
            sub.SubscriptionEventId = subEvent.SubscriptionEventId;

            ntsub = await unitOfWork.NotificationTypeSubscriptions.Get(x => x.SubscriptionId == subEvent.SubscriptionId);
            sub.NotificationTypeSubscriptionId = ntsub.NotificaitonTypeSubscriptionId;

            await unitOfWork.Save();
            return sub;
        }

        public async Task<ActionResult> Unsubscribe(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId) && userId == "") { return null; }

            var nts = await NotificationTypeSubscriptionExists(userId);
            var subscriptionEvent = await SubscriptionEventExists(eventId);

            ActionResult result;

            if (subscriptionEvent != null && nts != null)
            {
                await unitOfWork.NotificationTypeSubscriptions.Delete(nts.NotificaitonTypeSubscriptionId);
                // nts.SubscriptionId = 0;

                await unitOfWork.SubscriptionEvents.Delete(subscriptionEvent.SubscriptionEventId);
                await unitOfWork.Subscriptions.Delete(subscriptionEvent.SubscriptionId);

                await unitOfWork.Save();

                result = new NoContentResult();
                return result;
            }

            // if not above, then i haven't subbed

            result = new ConflictResult();
            return result;
        }

        public async Task<ICollection<Event>> ListOfSubscribedEvents(string userId)
        {
            if (userId != "")
            {
                var nts = await unitOfWork.NotificationTypeSubscriptions.Get(x => x.TelegramKey == userId);

                if (nts != null)
                {
                    var subs = await unitOfWork.Subscriptions.GetAll(x => x.NotificationTypeSubscriptionId == nts.NotificaitonTypeSubscriptionId, includes: new List<string> { "SubscriptionEvents" });

                    var subEvents = new List<SubscriptionEvent>();
                    foreach (var item in subs)
                    {
                        subEvents.AddRange(item.SubscriptionEvents);
                    }

                    var eventIds = new List<long>();
                    foreach (var item in subEvents)
                    {
                        eventIds.Add(item.EventId);
                    }
                    var subbedEvents = new List<Event>();
                    var events = await unitOfWork.Events.GetAll();

                    foreach (var @event in events)
                    {
                        foreach (var id in eventIds)
                        {
                            if (@event.EventId == id)
                            {
                                subbedEvents.Add(@event);
                            }
                        }
                    }
                    return subbedEvents;
                }
            }
            return null;
        }


        // How to validate seacrh from spaces, empty strings etc.?
        // Search using specific date format
        public async Task<ICollection<Event>> SearchEvents(string search)
        {
            DateTime dateTime;
            var date = DateTime.TryParseExact(search, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

            IList<Event> events = null;

            if (search != "" && !date)
            {
                events = await unitOfWork.Events.GetAll(
                        x => x.Title.Contains(search) ||
                        x.Description.Contains(search) ||
                        x.ShortDesc.Contains(search));
                    //x.StartAt.Date == Convert.ToDateTime(search))
                    //x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null))
            }
            else if (date) {
                events = await unitOfWork.Events.GetAll(
                    x => x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null));
            }

            return events;
        }

        // TODO: Irrelevant, should rewrite
        public async Task<ActionResult> SubscriptionExists(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId) && userId == "") { return null; }

            var nts = await NotificationTypeSubscriptionExists(userId);
            var subscriptionEvent = await SubscriptionEventExists(eventId);

            // ActionResult result;

            if (nts != null && subscriptionEvent != null)
            {
                // result = new ConflictResult();
                return null;
            }

            //result = new NoContentResult();
            return null;
        }

        public async Task<NotificationTypeSubscription> NotificationTypeSubscriptionExists(string userId)
        {
            if (userId != "") 
            {
                var nts = await unitOfWork.NotificationTypeSubscriptions.Get(x => x.TelegramKey == userId);
                if (nts != null)
                    return nts;

                return null;
            }
            return null;
        }

        public async Task<SubscriptionEvent> SubscriptionEventExists(long eventId)
        {
            if (eventId > 0)
            {
                var subEvent = await unitOfWork.SubscriptionEvents.Get(x => x.EventId == eventId);
                if (subEvent != null)
                    return subEvent;

                return null;
            }
            return null;
        }
    }
}

/* 
 * // TODO: Subscribe to Category,
 * filter Events based on Categories
 * Search Events by Title, Description
 * Sort Events
 * 
    передбачається, що наприклад категорії подій будуть додаватися спершу, тоді вже самі події
    коли користувач підписуватиметься, ви спершу створите його підписку, а тоді додасте підписку до якоїсь категорії подій



    Done: add category to event, 
          subscribe to event
 */

