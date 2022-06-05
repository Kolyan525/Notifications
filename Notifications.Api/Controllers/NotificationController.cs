using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notifications.BL.Services;
using System;
using System.Threading.Tasks;
using Notifications.BL.IRepository;
using Microsoft.EntityFrameworkCore;
using Notifications.BL.Services.Telegram;
using Notifications.BL;
using Notifications.DAL.Models.Telegram;
using Notifications.DAL.Models;
using Telegram.Bot;

namespace Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        readonly ILogger<NotificationController> logger;
        readonly NotificationsService notificationsService;
        readonly IUnitOfWork unitOfWork;
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        private EventActionActive TelActionEvent;

        public NotificationController(ILogger<NotificationController> logger, NotificationsService notificationsService, IUnitOfWork unitOfWork, TelegramBotClient botClient, IUserService userService, EventActionActive telActionEvent)
        {
            this.logger = logger;
            this.notificationsService = notificationsService;
            this.unitOfWork = unitOfWork;
            _botClient = botClient;
            _userService = userService;
            TelActionEvent = telActionEvent;
        }

        [HttpPost("fire-and-forget")]
        public IActionResult FireAndForget(string clientName)
        {
            string jobId = BackgroundJob.Enqueue(
                () => Console.WriteLine($"{clientName}, thank you for contacting us!"));

            return Ok($"Job ID: {jobId}");
        }

        [HttpPost("delayed")]
        public IActionResult Delayed(string clientName)
        {
            string jobId = BackgroundJob.Schedule(
                () => Console.WriteLine($"Session for client {clientName} has been closed!"), 
                TimeSpan.FromSeconds(60));

            return Ok($"Job ID: {jobId}");
        }

        [HttpPost("recurring")]
        public IActionResult Recurring()
        {
            RecurringJob.AddOrUpdate(
                () => Console.WriteLine("Happy Birthday!"), Cron.Daily);

            return Ok();
        }

        [HttpPost("continuations")]
        public IActionResult Continuations(string clientName)
        {
            string jobId = BackgroundJob.Enqueue(
                () => Console.WriteLine($"Check balance logic for {clientName}"));

            BackgroundJob.ContinueJobWith(jobId,
                () => Console.WriteLine($"{clientName}, your balance has been changed."));

            return Ok();
        }

        [HttpPost("recurring-notification")]
        public async Task<IActionResult> Notify()
        {
            // Sheck all events, if one is due, pass it to the function and retrieve list of subbed users.
            // Then send notification to the users
            // Schedule
            //var now = notificationsService.IsDue(60);

            string message = "Введіть назву події, яку бажаєте переглянути серед списку наявних подій! Для того, щоб ознайомитися із списком зі всіма подіями виберіть в меню варіант 'Events'!";
            
            await _botClient.SendTextMessageAsync(355735430, message);

            RecurringJob.AddOrUpdate<NotificationsService>(x => x.CheckEvents(new TimeSpan(0, 32, 0), new TimeSpan(0, 10, 0)), Cron.Minutely);

            return Ok();
        }

        [HttpPost("schedule-notification")]
        public IActionResult ScheduleNotification(TimeSpan span)
        {
            string jobId = BackgroundJob.Schedule(
                () => Console.WriteLine($"Schedule notification for client for {span}"),
                TimeSpan.FromMinutes(span.TotalMinutes));

            return Ok($"Job ID: {jobId}");
        }
    }
}