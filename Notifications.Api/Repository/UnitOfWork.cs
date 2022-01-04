using Notifications.Api.IRepository;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.Api.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly NotificationsContext context;
        IGenericRepository<Category> categories;
        IGenericRepository<Event> events;
        IGenericRepository<NotificationType> notificationTypes;
        public UnitOfWork(NotificationsContext context)
        {
            this.context = context;
        }
        public IGenericRepository<Category> Categories => categories ??= new GenericRepository<Category>(context);

        public IGenericRepository<Event> Events => events ??= new GenericRepository<Event>(context);

        public IGenericRepository<NotificationType> NotificationTypes => notificationTypes ??= new GenericRepository<NotificationType>(context);

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
