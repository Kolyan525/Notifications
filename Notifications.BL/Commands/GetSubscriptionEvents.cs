using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Notifications.DAL.Models.Telegram;
using Notifications.BL.Services.Telegram;
using Notifications.BL.IRepository;
using Notifications.DAL.Models;
using Notifications.BL.Services;
using Microsoft.EntityFrameworkCore;

namespace Notifications.BL.Commands
{
    public class GetSubscriptionEvents : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly NotificationsContext _context;
        private static List<SubscriptionEvent> SubscriptionEvents;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetSubscriptionEvents(IUnitOfWork unitOfWork, TelegramBot telegramBot, IUserService userService, NotificationsContext context, NotificationsService notificationsService)
        {
            _botClient = telegramBot.GetBot().Result;
            _userService = userService;
            _context = context;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
        }

        public override string Name => CommandNames.GetSubscriptionEvents;
        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            string allSubEvents = null;
            var id = user.ChatId;
            if (!_context.SubscriptionEvents.Any())
            {
                await _botClient.SendTextMessageAsync(id, "У вас не має подій на які ви підписані!");
            }
            //var sList = await unitOfWork.SubscriptionEvents.GetAll();
            //var sList = await notificationsService.ListOfSubscribedEvents(id.ToString());
            var sList = await unitOfWork.NotificationTypeSubscriptions.GetAllHere(
                nts => nts.TelegramKey == id.ToString(),
                include: nts => nts
                    .Include(nts => nts.Subscription)
                    .ThenInclude(s => s.SubscriptionEvents)
                    .ThenInclude(se => se.Event));

            var vents = new List<Notifications.DAL.Models.Event>();
            if (sList != null)
            {
                foreach (var b in sList)
                {
                    foreach (var se in b.Subscription.SubscriptionEvents)
                    {
                        vents.Add(se.Event);
                    }
                }
            }

            foreach (var Ev in vents)  
                allSubEvents += (Ev.Title + "\n");

            await _botClient.SendTextMessageAsync(id, allSubEvents);
        }
    }
}
