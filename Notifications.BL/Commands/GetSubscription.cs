using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.BL.IRepository;
using Notifications.BL.Services;

namespace Notifications.BL.Commands
{
    public class GetSubscription : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetSubscription(IUnitOfWork unitOfWork, TelegramBot telegramBot, NotificationsContext context, IUserService userService, NotificationsService notificationsService)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
        }

        public override string Name => CommandNames.GetSubscription;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var data = update.CallbackQuery.Data;
            var InlineKeyboardText = returnText(update.CallbackQuery.Message.Text);

            var EventList = await unitOfWork.Events.GetAll();
            if (EventList.Any())
            {
                foreach (var item in EventList)
                {
                    if (item.Title == InlineKeyboardText)
                    {
                        var check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                        if (check == false)
                        {
                            await notificationsService.SubscribeToEvent(item.EventId, id.ToString());
                            await _botClient.SendTextMessageAsync(id, "Ви успішно підписалися на подію: '" + InlineKeyboardText + "'");
                        }
                        else if (check == true)
                        {
                            await _botClient.SendTextMessageAsync(id, "Ви вже підписані на подію: '" + InlineKeyboardText + "'");
                        }
                    }
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                return;
            }
        }
        private static string returnText (string updateText)
        {
            string text = null;
            foreach(char i in updateText)
            {
                if (i == '.')
                    return text;
                text += i;
            }
            return text;
        }
    }
}
