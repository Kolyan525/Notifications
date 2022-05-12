using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.BL.IRepository;
using System.Linq;

namespace Notifications.BL.Commands
{
    public class GetEvents : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly TelegramBotClient _botClient;
        readonly IUnitOfWork unitOfWork;

        public GetEvents(IUserService userService, TelegramBot telegramBot, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
        }

        public override string Name => CommandNames.GetEvents;
        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            string allEvents = null;
            var id = user.ChatId;
            var EventList = await unitOfWork.Events.GetAll();
            if (EventList.Any())
            {
                foreach (var Ev in EventList)
                    allEvents += (Ev.Title + "\n");

                await _botClient.SendTextMessageAsync(id, allEvents);
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                return;
            }
        }
    }
}
