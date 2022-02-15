using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public NotificationsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ActionResult> GetEventCategories(long id)
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

                return ApiResponse.Ok(categories).ToResult();
            }

            return ApiResponse.BadRequest("Submitted data was invalid").ToResult();
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

        public async Task<ActionResult<Event>> AddCategoryToEvent(long eventId, long categoryId)
        {
            if (eventId > 0 && categoryId > 0)
            {
                var cat = await unitOfWork.Categories.Get(x => x.CategoryId == categoryId);
                if (cat == null) return ApiResponse.NotFound("Category was not found").ToResult();

                var @event = await unitOfWork.Events.Get(x => x.EventId == eventId, new List<string> { "EventCategories" });
                if (@event == null) return ApiResponse.NotFound("Event was not found").ToResult();

                // Check if EC already exists within event
                var ch = @event.EventCategories.FirstOrDefault(x => x.CategoryId == cat.CategoryId && x.Event.Title == @event.Title);
                if (ch != null)
                    return ApiResponse.Conflict("This category is already assigned to the event").ToResult();

                @event.EventCategories.Add(new EventCategory { CategoryId = cat.CategoryId });
                await unitOfWork.Save();

                return ApiResponse.Ok(@event).ToResult();
            }
            return ApiResponse.BadRequest("Submitted data was invalid").ToResult();
        }

        /// <summary>
        /// Subscribes to an event using telegram id and user id
        /// </summary>
        /// <param name="eventId">Event ID parameter, long</param>
        /// <param name="userId">Should be Telegram ID, string</param>
        /// <returns></returns>
        public async Task<ActionResult> Subscribe(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId)) return ApiResponse.NotFound("Event was not found").ToResult();
            if (string.IsNullOrEmpty(userId)) return ApiResponse.BadRequest("User ID is empty").ToResult();    // Should probably add validation

            var exists = SubscriptionExists(eventId, userId);
            if (exists.Result) return ApiResponse.Conflict("Event subscription already exists").ToResult();

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
            return ApiResponse.Ok("Succesfully subscribed!").ToResult();
        }

        public async Task<ActionResult> Unsubscribe(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId) && string.IsNullOrEmpty(userId)) { return ApiResponse.BadRequest("Submitted data was invalid").ToResult(); }

            var ntsubs = await unitOfWork.NotificationTypeSubscriptions.GetAll(x => x.TelegramKey == userId);
            var subEvents = await unitOfWork.SubscriptionEvents.GetAll(x => x.EventId == eventId);

            if (ntsubs == null) return ApiResponse.NotFound("No NTS was found").ToResult();
            if (subEvents == null) return ApiResponse.NotFound("No SubscriptionEvents was found").ToResult();

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

                        return ApiResponse.NoContent("Succesfuly unsubscribed").ToResult();
                    }
                }
            }

            // if not above, then i haven't subbed or nts/subEvent doesn't exist
            return ApiResponse.Conflict("Something went wrong. Probably, you aren't subscribed").ToResult();
        }

        public async Task<ActionResult> ListOfSubscribedEvents(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ApiResponse.BadRequest("Submitted data was invalid. Please try again!").ToResult();

            var subbed = await unitOfWork.NotificationTypeSubscriptions.GetAllHere(nts => nts.TelegramKey == userId,
                include: nts => nts.
                    Include(nts => nts.Subscription)
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
                return ApiResponse.Ok(vents).ToResult();
            }

            return ApiResponse.NotFound($"No subscriptions was found with following ID: \"{userId}\"").ToResult();
        }


        // How to validate seacrh from spaces, empty strings etc.?
        // Search using specific date format
        public async Task<ActionResult> SearchEvents(string search)
        {
            IList<Event> events = null;
            if (string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search)) return ApiResponse.BadRequest("Submitted data was invalid").ToResult();
            
            var date = DateTime.TryParseExact(search, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (!date)
            {
                events = await unitOfWork.Events.GetAll(
                        x => x.Title.Contains(search) ||
                        x.Description.Contains(search) ||
                        x.ShortDesc.Contains(search));
                //x.StartAt.Date == Convert.ToDateTime(search))
                //x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null))
            }
            else if (date)
            {
                events = await unitOfWork.Events.GetAll(
                    x => x.StartAt.Date == DateTime.ParseExact(search, "d.M.yyyy", null));
            }

            return ApiResponse.Ok(events).ToResult();
        }

        public async Task<bool> SubscriptionExists(long eventId, string userId)
        {
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
 * // TODO: Subscribe to Category,
 * filter Events based on Categories
 * Search Events by Title, Description
 * Sort Events
 * 
    передбачається, що наприклад категорії подій будуть додаватися спершу, тоді вже самі події
    коли користувач підписуватиметься, ви спершу створите його підписку, а тоді додасте підписку до якоїсь категорії подій



    Done: search event, 
          subscribe to event, unsubscribe to event,
          get list of subbed events
 */

