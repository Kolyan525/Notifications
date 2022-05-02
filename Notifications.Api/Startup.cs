using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.BL.IRepository;
using Notifications.BL.Repository;
using Notifications.BL.Services;
using Notifications.DAL.DbInitializer;
using Notifications.DAL.Models;
using Notifications.DTO.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Notifications.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(op => {
                    op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    op.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<NotificationsContext>(options =>
            {
                options.UseSqlServer(connection);
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Scoped);

            services.AddHangfire(h => h.UseSqlServerStorage(connection));
            services.AddHangfireServer();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/account/google-login";
                })
                .AddGoogle(options =>
                {
                    options.ClientId = "629075882388-da8gmv28t2tfe4pegh4mt47lpt0r91md.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                });

            services.ConfigureIdentity();
            //services.ConfigureJWT(Configuration);

            services.AddCors(o =>
            {
                o.AddPolicy("AllowAll", builder =>
                    builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitializer));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<NotificationsService>();
            services.AddTransient<DbInitializer>();
            services.AddScoped<IAuthManager, AuthManager>();

            //services.AddControllersWithViews().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            //});

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationsApi", Version = "v1" });
            //    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            //    c.AddSecurityDefinition("Bearer",
            //          new OpenApiSecurityScheme
            //          {
            //              Description =
            //              //"JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            //              "Put **_ONLY_** your JWT Bearer token on textbox below!",
            //              Name = "Authorization",
            //              In = ParameterLocation.Header,
            //              //Type = SecuritySchemeType.ApiKey,
            //              Type = SecuritySchemeType.Http,
            //              Scheme = "Bearer"
            //          });
            //      c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            //      {
            //          {
            //              new OpenApiSecurityScheme
            //              {
            //                  Reference = new OpenApiReference
            //                  {
            //                      Type = ReferenceType.SecurityScheme,
            //                      Id = "Bearer"
            //                  },
            //                  Scheme = "oauth2",
            //                  Name = "Bearer",
            //                  In = ParameterLocation.Header,

            //              },
            //              new List<string>()
            //          }
            //      });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //app.UseSwagger();
                //app.UseSwagger(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationsDAL v1"));
                //app.UseSwagger();
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notifications API V1");
                //});
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/dashboard");

            app.UseCors(opt => opt.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Events}/{action=GetEvents}/{id?}");
            });
        }
    }
}
