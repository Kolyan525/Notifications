using System.Collections.Generic;
using System.Threading.Tasks;
using Notifications.DAL.Models;

namespace Notifications.BL.Services.Telegram
{
    public interface IPaginationService
    {
        void SetPaginationList(IList<Event> events);
        Task<IList<Event>> GetPagination();
        Task ClearCache();
    }
}
