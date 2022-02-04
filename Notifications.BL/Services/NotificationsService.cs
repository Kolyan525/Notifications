using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.BL.IRepository;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
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

        public async Task<Subscription> Subscribe(long eventId, string userId)
        {
            if (!unitOfWork.Events.Exists(eventId)) { return null; }

            var @event = await unitOfWork.Events.Get(x => x.EventId == eventId);

            if (@event == null) return null;
            if (userId == null) return null;

            var nts = new NotificationTypeSubscription
            {
                TelegramKey = userId,
            };

            var subscription = new Subscription
            {
                NotificationTypeSubscriptions = new List<NotificationTypeSubscription> { nts },
                SubscriptionEvents = new List<SubscriptionEvent> { new SubscriptionEvent { Event = @event } }
            };

            await unitOfWork.Subscriptions.Insert(subscription);
            await unitOfWork.Save();

            return subscription;
        }
    }
}

/* 
 * // TODO: Subscribe to Event/Category,
 * filter Events based on Categories
 * Search Events by Title, Description
 * Sort Events
 * 
    зробіть робочу апі і через нього додавайте данні
    передбачається, що наприклад категорії подій будуть додаватися спершу, тоді вже самі події
    коли користувач підписуватиметься, ви спершу створите його підписку, а тоді додасте підписку до якоїсь категорії подій



    Done: add category to event
 */

