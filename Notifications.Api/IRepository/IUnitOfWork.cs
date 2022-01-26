using Notifications.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Notifications.Api.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Event> Events { get; }
        IGenericRepository<NotificationType> NotificationTypes { get; }
        Task Save();
    }
}
