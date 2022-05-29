using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
    }
}
