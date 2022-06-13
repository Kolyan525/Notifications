using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notifications.BL.IRepository;

namespace Notifications.BL.Services.Telegram
{
    public class CacheService : ICacheService
    {
        private IMemoryCache cache;
        IUnitOfWork unitOfWork;

        public CacheService(IMemoryCache memoryCache, IUnitOfWork unitOfWork)
        {
            cache = memoryCache;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IList<Event>> GetPagination()
        {
            Event @event = null;
            IList<Event> paginationList = new List<Event>();
            var Eventlist = await unitOfWork.Events.GetAll();
            if (Eventlist.Any())
            {
                foreach (var item in Eventlist)
                {
                    @event = item;
                    if (cache.TryGetValue(@event.EventId, out @event))
                        paginationList.Add(@event);
                }
            }
            return paginationList;
        }

        public void SetPaginationList(IList<Event> events)
        {
            foreach (var item in events)
            {
                cache.Set(item.EventId, item, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
        }

        public async Task<bool> CheckActiveCacheEvent()
        {
            //if (cache.TryGetValue("ActiveEventID", out ActiveCacheEventClass @activeCacheEvent))
            //    return true;
            var events = await unitOfWork.Events.GetAll();
            foreach (var @event in events)
                if (cache.TryGetValue(@event.EventId.ToString() + "Активовано", out string value))
                    return true;

            return false;
        }
        public void ActiveCacheEvent(string id, string eventInCache = "Активовано")
        {
            if (!cache.TryGetValue(id, out string value))
            {
                cache.Set(id, eventInCache, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
        }
        public void SetNotificationForEvent(string id, DateTime timeToEvent)
        {
            //TimeToEvent timeToEventObject = new TimeToEvent();
            //timeToEventObject.Id = timeToEvent.Millisecond;
            //timeToEventObject.timeToEvent = timeToEvent;
            cache.Set(id, timeToEvent, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }
        public void SetEventToCache(string id, Event @event)
        {
            if (!cache.TryGetValue(id, out Event ev))
            {
                cache.Set(id, @event, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
        }
        public async Task<Event> GetEventFromCache()
        {
            Event @event = null;
            var events = await unitOfWork.Events.GetAll();
            foreach (var item in events)
                if (cache.TryGetValue(item.EventId.ToString() + "Кеш", out @event))
                    return @event;

            return @event;
        }
        public async Task<DateTime> GetNotificationForEvent()
        {
            DateTime timeToEvent = DateTime.Now;
            var events = await unitOfWork.Events.GetAll();
            foreach (var @event in events)
                if (cache.TryGetValue(@event.EventId.ToString() + "userNotification", out timeToEvent))
                    return timeToEvent;

            return timeToEvent;
        }
        public async Task ClearPaginationListCache()
        {
            Event @event = null;
            var list = await unitOfWork.Events.GetAll();
            foreach (var item in list)
            {
                @event = item;
                if (cache.TryGetValue(@event.EventId, out @event))
                    cache.Remove(@event.EventId);
            }
        }
        public async Task CleaNotificationForEventrCache()
        {
            Event @event = null;
            var list = await unitOfWork.Events.GetAll();
            foreach (var item in list)
            {
                @event = item;
                if (cache.TryGetValue(@event.EventId.ToString() + "userNotification", out DateTime timeToEvent))
                    cache.Remove(@event.EventId.ToString() + "userNotification");
            }
        }
        public async Task ClearActiveCacheEventCache()
        {
            Event @event = null;
            var list = await unitOfWork.Events.GetAll();
            foreach (var item in list)
            {
                @event = item;
                if (cache.TryGetValue(@event.EventId.ToString() + "Активовано", out string value))
                    cache.Remove(@event.EventId.ToString() + "Активовано");

            }
        }
        public async Task ClearEventFromCache()
        {
            Event @event = null;
            var list = await unitOfWork.Events.GetAll();
            foreach (var item in list)
            {
                @event = item;
                if (cache.TryGetValue(@event.EventId.ToString() + "Кеш", out Event ev))
                    cache.Remove(@event.EventId.ToString() + "Кеш");
            }
        }
    }
    //public class ActiveCacheEventClass
    //{
    //    public string Id = "ActiveEventID";
    //    public string ActiveEvent = "Активовано";
    //}
}
