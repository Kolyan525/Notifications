using Notifications.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Notifications.BL.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Event> Events { get; }
        IGenericRepository<NotificationType> NotificationTypes { get; }
        IGenericRepository<EventCategory> EventCategories { get; }
        IGenericRepository<NotificationTypeSubscription> NotificationTypeSubscriptions { get; }
        IGenericRepository<Subscription> Subscriptions { get; }
        IGenericRepository<SubscriptionEvent> SubscriptionEvents { get; }
        IGenericRepository<ApplicationUser> Users { get; }
        Task Save();
    }
}
