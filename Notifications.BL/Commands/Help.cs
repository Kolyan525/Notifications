using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Notifications.BL.Services.Telegram;

namespace Notifications.BL.Commands
{
    public class Help : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;

        public Help(TelegramBot telegramBot, IUserService userService)
        {
            _botClient = telegramBot.GetBot().Result;
            _userService = userService;
        }

        public override string Name => CommandNames.Help;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            string Commands = "help - отримати список всіх команд.\nEvents - отримати список всіх подій.\nSubEvents - отримати списк всіх подій на які ви підписані.\nEvent - отримати інформацію про конкретну подію.";
            await _botClient.SendTextMessageAsync(id, Commands);
        }
    }
}
