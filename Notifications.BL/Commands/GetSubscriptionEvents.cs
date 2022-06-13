using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.BL.IRepository;
using Notifications.DAL.Models;
using Notifications.BL.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace Notifications.BL.Commands
{
    public class GetSubscriptionEvents : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly NotificationsContext _context;
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

            var id = user.ChatId;
            if (!_context.SubscriptionEvents.Any())
            {
                await _botClient.SendTextMessageAsync(id, "У вас не має подій на які ви підписані!");
                return;
            }
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
            var inlineKeyboardDetail = TelegramButtons.GetSubscriptionEvents.Detail;
            var buttons = TelegramButtons.GetSubscriptionEvents.Buttons;
            int i = 0;
            if (update.Type == UpdateType.Message)
            {
                foreach (var Ev in vents)
                {
                    if (i == 2 && Ev != vents.Last())
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                        replyMarkup: buttons, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        return;
                    }
                    await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                        replyMarkup: inlineKeyboardDetail, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    i++;
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                string text = "";
                foreach (var ch in update.CallbackQuery.Message.Text)
                {
                    if (ch == '\n')
                    {
                        bool chek = vents.FirstOrDefault(x => x.Title == text).Title == text;
                        if (chek == true)
                            break;
                    }
                    text += ch;
                }
                var lastEvent = vents.FirstOrDefault(e => e.Title == text);
                List<DAL.Models.Event> NewEventList = new List<DAL.Models.Event>();
                int k = 0;
                foreach (var ev in vents)
                {
                    if (k > 0)
                    {
                        NewEventList.Add(ev);
                    }
                    if (ev == lastEvent)
                        k++;
                }
                if (NewEventList.Count == 0)
                {
                    return;
                }
                else
                {
                    foreach (var Ev in NewEventList)
                    {
                        if (i == 2 && Ev != NewEventList.Last())
                        {
                            await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                            replyMarkup: buttons, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                            return;
                        }
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                            replyMarkup: inlineKeyboardDetail, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        i++;
                    }
                }
            }
            return;
        }
    }
}
