using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notifications.DAL.DbInitializer;
using Serilog;
using Serilog.Events;
using System;
using X.Extensions.Logging.Telegram;

namespace Notifications.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* var options = new TelegramLoggerOptions
            {
                AccessToken = "5039937835:AAGz2JqiF3S8SKuqs_6UdRMX-p4nQ86K00U",
                ChatId = "-1001669767630",
                LogLevel = LogLevel.Information,
                Source = "Notifications App"
            };

            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    .AddTelegram(options)
                    .AddConsole();
            }
            );

            CreateHostBuilder(args).Build().Run();
            */

            var host = CreateHostBuilder(args).Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: $"{Environment.CurrentDirectory}\\Logs\\log-.txt",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .WriteTo.Console().CreateLogger();
            try
            {
                Log.Information("Notifications App Is Starting");

                SeedDatabase(host);

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Notifications App failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureLogging((context, builder) =>
                {
                    if (context.Configuration != null)
                        builder
                            .AddTelegram(context.Configuration)
                            .AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void SeedDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                    context.Initialize().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database");
                }
            }
        }

        /*
        public class ExampleClass
        {
        }

        public class AnotherExampleClass
        {
        }

        static void Main(string[] args)
        {
            var options = new TelegramLoggerOptions
            {
                AccessToken = "5039937835:AAGz2JqiF3S8SKuqs_6UdRMX-p4nQ86K00U",
                ChatId = "-1001669767630",
                LogLevel = LogLevel.Information,
                Source = "TEST APP",
                UseEmoji = true
            };

            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    .AddTelegram(options)
                    .AddConsole();
            }
            );

            var logger1 = factory.CreateLogger<ExampleClass>();
            var logger2 = factory.CreateLogger<AnotherExampleClass>();

            for (var i = 0; i < 1; i++)
            {
                logger1.LogTrace($"Message {i}");
                logger2.LogDebug($"Debug message text {i}");
                logger1.LogInformation($"Information message text {i}");

                try
                {
                    throw new SystemException("Exception message description. <br /> This message contains " +
                                              "<html> <tags /> And some **special** symbols _");
                }
                catch (Exception exception)
                {
                    logger2.LogWarning(exception, $"Warning message text {i}");
                    logger1.LogError(exception, $"Error message  text {i}");
                    logger2.LogCritical(exception, $"Critical error message  text {i}");
                }

                Task.WaitAll(Task.Delay(500));
            }


            Console.WriteLine("Hello World!");
        }
        */
    }
}
