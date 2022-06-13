using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.DAL.Models;

namespace Notifications.BL.Services.Telegram
{
    public interface ICacheService
    {
        void SetPaginationList(IList<Event> events);
        Task<IList<Event>> GetPagination();
        void SetNotificationForEvent(string id, DateTime timeToEvent);
        void ActiveCacheEvent(string id, string eventInCache = "Активовано");
        Task<DateTime> GetNotificationForEvent();
        void SetEventToCache(string id, Event @event);
        Task<Event> GetEventFromCache();
        Task<bool> CheckActiveCacheEvent();
        Task ClearPaginationListCache();
        Task CleaNotificationForEventrCache();
        Task ClearActiveCacheEventCache();
        Task ClearEventFromCache();
    }
}
