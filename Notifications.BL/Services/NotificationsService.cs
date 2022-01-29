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
    }
}

/* 
 * // TODO: Subscribe to Event/Category,
 * filter Events based on Categories
 * Search Events by Title, Description
 * Sort Events
 */

