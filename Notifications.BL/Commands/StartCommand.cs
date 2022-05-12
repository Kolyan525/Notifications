using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Notifications.BL.Services.Telegram;

namespace Notifications.BL.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;

        public StartCommand(IUserService userService, TelegramBot telegramBot)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            ReplyKeyboardMarkup replyKeyboard = new (new[]
            {
                new KeyboardButton[] {"help", "Events"},
                new KeyboardButton[] { "Event", "SubEvents" }
            })
            {
                ResizeKeyboard = true
            };

            await _botClient.SendTextMessageAsync(user.ChatId, "Вітаємо! Ви запустили нашого телеграм-бота! Для того, щоб вивести список команд введіть 'help', або ж оберіть цей варіант у меню! ",
                ParseMode.Markdown, replyMarkup: replyKeyboard);

        }
    }
}
