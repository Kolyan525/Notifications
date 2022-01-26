using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Notifications.DAL.Models;

namespace Notifications.Api
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApplicationUser>(q => q.User.RequireUniqueEmail = true);

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder
                .AddEntityFrameworkStores<NotificationsContext>()
                .AddDefaultTokenProviders();
        }
    }
}
