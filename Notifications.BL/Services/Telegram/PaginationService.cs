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
    public class PaginationService : IPaginationService
    {
        private IMemoryCache cache;
        IUnitOfWork unitOfWork;

        public PaginationService(IMemoryCache memoryCache, IUnitOfWork unitOfWork)
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
                    {
                        paginationList.Add(@event);
                    }
                }
            }
            return paginationList;
        }

        public void SetPaginationList(IList<Event> events)
        {
            //await ClearCache();
            foreach (var item in events)
            {
                cache.Set(item.EventId, item, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
        }
        public async Task ClearCache()
        {
            Event @event = null;
            var list = await unitOfWork.Events.GetAll();
            foreach (var item in list)
            {
                @event = item;
                if(cache.TryGetValue(@event.EventId, out @event))
                    cache.Remove(@event.EventId);
            }
        }
    }
}
