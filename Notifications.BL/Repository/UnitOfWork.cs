using Notifications.BL.IRepository;
using Notifications.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Notifications.BL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly NotificationsContext context;
        IGenericRepository<Category> categories;
        IGenericRepository<Event> events;
        IGenericRepository<EventCategory> eventCategories;
        IGenericRepository<NotificationType> notificationTypes;
        IGenericRepository<NotificationTypeSubscription> notificationTypeSubscriptions;
        IGenericRepository<Subscription> subscriptions;
        IGenericRepository<SubscriptionEvent> subscriptionEvents;
        IGenericRepository<ApplicationUser> users;
        IGenericRepository<RefreshToken> refreshTokens;
        public UnitOfWork(NotificationsContext context)
        {
            this.context = context;
        }
        public IGenericRepository<Category> Categories => categories ??= new GenericRepository<Category>(context);

        public IGenericRepository<Event> Events => events ??= new GenericRepository<Event>(context);

        public IGenericRepository<EventCategory> EventCategories => eventCategories ??= new GenericRepository<EventCategory>(context);

        public IGenericRepository<NotificationType> NotificationTypes => notificationTypes ??= new GenericRepository<NotificationType>(context);

        public IGenericRepository<NotificationTypeSubscription> NotificationTypeSubscriptions => notificationTypeSubscriptions ??= new GenericRepository<NotificationTypeSubscription>(context);
        
        public IGenericRepository<Subscription> Subscriptions => subscriptions ??= new GenericRepository<Subscription>(context);
        
        public IGenericRepository<SubscriptionEvent> SubscriptionEvents => subscriptionEvents ??= new GenericRepository<SubscriptionEvent>(context);

        public IGenericRepository<ApplicationUser> Users => users ??= new GenericRepository<ApplicationUser>(context);
        
        public IGenericRepository<RefreshToken> RefreshTokens => refreshTokens ??= new GenericRepository<RefreshToken>(context);

        public void Dispose()
        {
            context.Dispose();  // Dispose of all the memory context was using
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
