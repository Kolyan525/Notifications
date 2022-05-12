using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Notifications.DAL.Models;
using Notifications.DAL.Models.Telegram;
using Notifications.BL.Services.Telegram;
using System.Linq;

namespace Notifications.BL.Commands
{
    public class Event : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly NotificationsContext _context;
        private EventActionActive TelActionEvent;

        public Event(IUserService userService, TelegramBot telegramBot, NotificationsContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            _context = context;
        }

        public override string Name => CommandNames.Event;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);

            string message = "Введіть назву події, яку бажаєте переглянути серед списку наявних подій! Для того, щоб ознайомитися із списком зі всіма подіями виберіть в меню варіант 'Events'!";
            var id = user.ChatId;
            await _botClient.SendTextMessageAsync(id, message);
            if (!_context.telegramEvent.Any())
            {
                string ev = "Подія";
                TelActionEvent = new EventActionActive();
                TelActionEvent.EventOption = ev;
                await _context.telegramEvent.AddAsync(TelActionEvent);
                await _context.SaveChangesAsync();
            }
        }
    }
}
