using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Notifications.BL.Commands;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using System;
using System.Text;

namespace Notifications.Api
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApplicationUser>(
                q =>
                {
                    q.User.RequireUniqueEmail = true;
                });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder
                .AddEntityFrameworkStores<NotificationsContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
        {
            var jwtSettings = Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KEY"));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                //ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = false, // TODO: change in prod
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                op.SaveToken = true;
                op.TokenValidationParameters = tokenValidationParameters;
            });
        }

        public static void ConfigureTelegramBot(this IServiceCollection services)
        {
            services.AddSingleton<TelegramBot>();
            services.AddScoped<ICommandExecutor, CommandExecutor>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<BaseCommand, StartCommand>();
            services.AddScoped<BaseCommand, BL.Commands.Event>();
            services.AddScoped<BaseCommand, GetEvent>();
            services.AddScoped<BaseCommand, Help>();
            services.AddScoped<BaseCommand, GetEvents>();
            services.AddScoped<BaseCommand, GetSubscriptionEvents>();
            services.AddScoped<BaseCommand, GetSubscription>();
            services.AddScoped<BaseCommand, GetUnsubscribe>();
            services.AddScoped<BaseCommand, GetCategories>();
            services.AddScoped<BaseCommand, GetSubscriptionCategory>();
            services.AddScoped<BaseCommand, GetUnsubscribeCategory>();
            services.AddScoped<BaseCommand, GetNotifications>();
        }
    }
}
