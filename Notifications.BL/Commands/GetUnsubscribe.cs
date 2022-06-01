using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.BL.IRepository;
using Notifications.BL.Services;

namespace Notifications.BL.Commands
{
    public class GetUnsubscribe : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetUnsubscribe(TelegramBot telegramBot, NotificationsContext context, IUserService userService, NotificationsService notificationsService, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.notificationsService = notificationsService;
            this.unitOfWork = unitOfWork;
        }

        public override string Name => CommandNames.GetUnsubscribe;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var data = update.CallbackQuery.Data;
            var InlineKeyboardText = returnText(update.CallbackQuery.Message.Text);

            var Events = await unitOfWork.Events.GetAll();
            if (Events.Any())
            {
                DAL.Models.Event ev = getEvent(Events, InlineKeyboardText);
                var check = notificationsService.SubscriptionExists(ev.EventId, id.ToString()).Result;
                if (check == true)
                {
                    foreach (var item in Events)
                    {

                        if (item.Title == InlineKeyboardText)
                        {
                            await notificationsService.UnsubscribeFromEvent(item.EventId, id.ToString());
                            await _botClient.SendTextMessageAsync(id, "Ви відписалися від події: '" + InlineKeyboardText + "'");
                        }
                    }
                    return;
                }
                else if (check == false)
                {
                    await _botClient.SendTextMessageAsync(id, "Для того, щоб відписатися від події '" + InlineKeyboardText + "' спочатку підпишіться на неї");
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                return;
            }
        }
        private static DAL.Models.Event getEvent(IList<DAL.Models.Event> Events, string InlineKeyboardText)
        {
            foreach (var i in Events)
            {
                if (i.Title == InlineKeyboardText)
                    return i;
            }
            return null;
        }
        private static string returnText(string updateText)
        {
            string text = null;
            foreach (char i in updateText)
            {
                if (i == '\n')
                    return text;
                text += i;
            }
            return text;
        }
    }
}
