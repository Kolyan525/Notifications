using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Notifications.DAL.Models.Telegram;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.BL.IRepository;
using Notifications.BL.Services;

namespace Notifications.BL.Commands
{
    public class GetEvent : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetEvent(TelegramBot telegramBot, NotificationsContext context, IUserService userService, IUnitOfWork unitOfWork, NotificationsService notificationsService)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
        }

        public override string Name => CommandNames.GetEvent;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            EventActionActive TelActionEvent;
            var EventList = await unitOfWork.Events.GetAll();
            string text = string.Empty;
            string category = string.Empty;
            
            if (EventList.Any())
            {

                InlineKeyboardMarkup inlineKeyboardSubscription = new(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "Subscription")
                }
            });
                InlineKeyboardMarkup inlineKeyboardUnsubscribe = new(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "Unsubscribe")
                }
            });

                if(update.Type == UpdateType.CallbackQuery)
                {
                    foreach (var ch in update.CallbackQuery.Message.Text)
                    {
                        if (ch == '\n')
                        {
                            bool chek = EventList.FirstOrDefault(x => x.Title == text).Title == text;
                            if (chek == true)
                                break;
                        }
                        text += ch;
                    }
                }
                else if(update.Type == UpdateType.Message)
                {
                    text = update.Message.Text;
                }
                else
                {
                    return;
                }

                foreach (var item in EventList)
                {
                    if (item.Title == text)
                    {
                        var response = $"<u><b>{item.Title}</b></u>\n<b>Опис події:</b> {item.Description}.";
                        if (_context.SubscriptionEvents.Any())
                        {
                            var check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                            if (check == true)
                            {
                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                TelActionEvent = _context.TelegramEvent.SingleOrDefault(x => x == _context.TelegramEvent.FirstOrDefault());
                                _context.TelegramEvent.Remove(TelActionEvent);
                                await _context.SaveChangesAsync();
                                return;
                            }
                        }
                        await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                        TelActionEvent = _context.TelegramEvent.SingleOrDefault(x => x == _context.TelegramEvent.FirstOrDefault());
                        _context.TelegramEvent.Remove(TelActionEvent);
                        await _context.SaveChangesAsync();
                        return;
                    }
                }
                await _botClient.SendTextMessageAsync(id, "Не має такої події!");
                TelActionEvent = _context.TelegramEvent.SingleOrDefault(x => x == _context.TelegramEvent.FirstOrDefault());
                _context.TelegramEvent.Remove(TelActionEvent);
                await _context.SaveChangesAsync();
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                TelActionEvent = _context.TelegramEvent.SingleOrDefault(x => x == _context.TelegramEvent.FirstOrDefault());
                _context.TelegramEvent.Remove(TelActionEvent);
                await _context.SaveChangesAsync();
                return;
            }

        }
    }
}
