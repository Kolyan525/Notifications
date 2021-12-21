using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using X.Extensions.Logging.Telegram;
using Microsoft.Extensions.Options;

namespace Notifications.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var options = new TelegramLoggerOptions
            //{
            //    AccessToken = "5039937835:AAGz2JqiF3S8SKuqs_6UdRMX-p4nQ86K00U",
            //    ChatId = "-1001669767630",
            //    LogLevel = LogLevel.Information,
            //    Source = "Notifications App"
            //};

            //var factory = LoggerFactory.Create(builder =>
            //{
            //    builder
            //        .ClearProviders()
            //        .AddTelegram(options)
            //        .AddConsole();
            //}
            //);

            //CreateHostBuilder(args).Build().Run();

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
                CreateHostBuilder(args).Build().Run();
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

        //public class ExampleClass
        //{
        //}

        //public class AnotherExampleClass
        //{
        //}

        //static void Main(string[] args)
        //{
        //    var options = new TelegramLoggerOptions
        //    {
        //        AccessToken = "5039937835:AAGz2JqiF3S8SKuqs_6UdRMX-p4nQ86K00U",
        //        ChatId = "-1001669767630",
        //        LogLevel = LogLevel.Information,
        //        Source = "TEST APP",
        //        UseEmoji = true
        //    };

        //    var factory = LoggerFactory.Create(builder =>
        //    {
        //        builder
        //            .ClearProviders()
        //            .AddTelegram(options)
        //            .AddConsole();
        //    }
        //    );

        //    var logger1 = factory.CreateLogger<ExampleClass>();
        //    var logger2 = factory.CreateLogger<AnotherExampleClass>();

        //    for (var i = 0; i < 1; i++)
        //    {
        //        logger1.LogTrace($"Message {i}");
        //        logger2.LogDebug($"Debug message text {i}");
        //        logger1.LogInformation($"Information message text {i}");

        //        try
        //        {
        //            throw new SystemException("Exception message description. <br /> This message contains " +
        //                                      "<html> <tags /> And some **special** symbols _");
        //        }
        //        catch (Exception exception)
        //        {
        //            logger2.LogWarning(exception, $"Warning message text {i}");
        //            logger1.LogError(exception, $"Error message  text {i}");
        //            logger2.LogCritical(exception, $"Critical error message  text {i}");
        //        }

        //        Task.WaitAll(Task.Delay(500));
        //    }


        //    Console.WriteLine("Hello World!");
        //}
    }
}
