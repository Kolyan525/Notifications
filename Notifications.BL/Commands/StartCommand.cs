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
                new KeyboardButton[] {"Список команд", "Події"},
                new KeyboardButton[] { "Подія", "Підписані події" },
                new KeyboardButton[] { "Категорії" }
            })
            {
                ResizeKeyboard = true
            };

            await _botClient.SendTextMessageAsync(user.ChatId, "Вітаємо! Ви запустили нашого телеграм-бота! Для того, щоб переглянути список команд введіть 'help', або ж оберіть цей варіант у меню! ",
                ParseMode.Markdown, replyMarkup: replyKeyboard);

        }
    }
}
