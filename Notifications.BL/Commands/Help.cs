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
            string Commands = "Список команд - отримати список всіх команд.\nПодії - отримати список всіх подій." +
                "\nВідстежувані події - отримати списк всіх подій на які ви підписані." +
                "\nПодія - отримати інформацію про конкретну подію." +
                "\nКатегорії - отримати список всіх категорій і подій, які відносяться до них.";
            await _botClient.SendTextMessageAsync(id, Commands);
        }
    }
}
