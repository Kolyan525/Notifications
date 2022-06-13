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
using System.Collections.Generic;

namespace Notifications.BL.Commands
{
    public class GetEvent : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;
        readonly ICacheService paginationService;

        public GetEvent(TelegramBot telegramBot, NotificationsContext context, IUserService userService, IUnitOfWork unitOfWork, NotificationsService notificationsService, ICacheService paginationService)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
            this.paginationService = paginationService;
        }

        public override string Name => CommandNames.GetEvent;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var EventList = await unitOfWork.Events.GetAll();
            string text = string.Empty;
            string category = string.Empty;
            int k = 0, i = 0;

            if (EventList.Any())
            {
                var inlineKeyboardSubscription = TelegramButtons.GetEvent.Subscription;
                var inlineKeyboardUnsubscribe = TelegramButtons.GetEvent.Unsubscribe;
                var buttonsWithSubscription = TelegramButtons.GetEvent.ButtonsWithSubscription;
                var buttonsWithUnsubscribe = TelegramButtons.GetEvent.ButtonsWithUnsubscribe;

                if (update.Type == UpdateType.CallbackQuery)
                {
                    if (update.CallbackQuery.Data.Contains("Detail"))
                    {
                        text = GetEventFromText(update, EventList);
                    }
                    else if (update.CallbackQuery.Data.Contains("NextSearchingEvents"))
                    {
                        var list = paginationService.GetPagination().Result;
                        if (list.Any())
                        {
                            string CallbackQueryText = GetEventFromText(update, EventList);
                            DAL.Models.Event lastEvent = null;
                            lastEvent = EventList.FirstOrDefault(e => e.Title == CallbackQueryText);
                            if (lastEvent != null)
                            {
                                foreach (var item in list)
                                {
                                    bool check = false;
                                    var response = $"<u><b>{item.Title}</b></u>\n\n<b>Опис події:</b> {item.Description}.";
                                    if (item.EventLink != string.Empty)
                                        response += $"\n\n<b>Посилання:</b> {item.EventLink}";
                                    if (item.Location != string.Empty)
                                        response += $"\n\n<b>Місце проведення:</b> {item.Location}";
                                    if (item.Price > 0)
                                        response += $"\n\n<b>Ціна:</b> {item.Price}";
                                    response += $"\n\n<b>Початок:</b> {item.StartAt.ToLocalTime()}";

                                    if (k > 0)
                                    {
                                        if (_context.SubscriptionEvents.Any())
                                        {
                                            check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                                            if (check == true)
                                            {
                                                if (i == 1 && item != list.Last())
                                                {
                                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: buttonsWithUnsubscribe, parseMode: ParseMode.Html);
                                                    await changeDatabase();
                                                    return;
                                                }
                                                else if (item == list.Last())
                                                {
                                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                                    await changeDatabase();
                                                    await paginationService.ClearPaginationListCache();
                                                    return;
                                                }
                                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                                await changeDatabase();
                                            }
                                        }
                                        if (check == false)
                                        {
                                            if (i == 1 && item != list.Last())
                                            {
                                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: buttonsWithSubscription, parseMode: ParseMode.Html);
                                                await changeDatabase();
                                                return;
                                            }
                                            else if (item == list.Last())
                                            {
                                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                                                await changeDatabase();
                                                await paginationService.ClearPaginationListCache();
                                                return;
                                            }
                                            await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                                            await changeDatabase();
                                        }
                                        i++;
                                    }
                                    if (item.Title == lastEvent.Title)
                                        k++;
                                }
                            }
                            return;
                        }
                    }
                }
                else if (update.Type == UpdateType.Message)
                {
                    text = update.Message.Text;
                }
                else
                {
                    return;
                }

                var reslut = await notificationsService.SearchEvents(text);
                var events = reslut.Data;

                if (events.Any())
                {
                    if (events.Count == 1)
                    {
                        foreach (var item in events)
                        {
                            bool check = false;
                            var response = $"<u><b>{item.Title}</b></u>\n\n<b>Опис події:</b> {item.Description}.";
                            if (item.EventLink != string.Empty)
                                response += $"\n\n<b>Посилання:</b> {item.EventLink}";
                            if (item.Location != string.Empty)
                                response += $"\n\n<b>Місце проведення:</b> {item.Location}";
                            if (item.Price > 0)
                                response += $"\n\n<b>Ціна:</b> {item.Price}";
                            response += $"\n\n<b>Початок:</b> {item.StartAt.ToLocalTime()}";

                            if (_context.SubscriptionEvents.Any())
                            {
                                check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                                if (check == true)
                                {
                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                    await changeDatabase();
                                    return;
                                }
                            }
                            if (check == false)
                            {
                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                                await changeDatabase();
                                return;
                            }
                        }
                    }
                    else
                    {
                        paginationService.SetPaginationList(events);
                        foreach (var item in events)
                        {
                            bool check = false;
                            var response = $"<u><b>{item.Title}</b></u>\n\n<b>Опис події:</b> {item.Description}.";
                            if (item.EventLink != string.Empty)
                                response += $"\n\n<b>Посилання:</b> {item.EventLink}";
                            if (item.Location != string.Empty)
                                response += $"\n\n<b>Місце проведення:</b> {item.Location}";
                            if (item.Price > 0)
                                response += $"\n\n<b>Ціна:</b> {item.Price}";
                            response += $"\n\n<b>Початок:</b> {item.StartAt.ToLocalTime()}";

                            if (_context.SubscriptionEvents.Any())
                            {
                                check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                                if (check == true)
                                {
                                    if (i == 1 && item != events.Last())
                                    {
                                        await _botClient.SendTextMessageAsync(id, response, replyMarkup: buttonsWithUnsubscribe, parseMode: ParseMode.Html);
                                        await changeDatabase();
                                        return;
                                    }
                                    else if (item == events.Last())
                                    {
                                        await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                        await changeDatabase();
                                        await paginationService.ClearPaginationListCache();
                                        return;
                                    }
                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardUnsubscribe, parseMode: ParseMode.Html);
                                    await changeDatabase();
                                }
                            }
                            if (check == false)
                            {
                                if (i == 1 && item != events.Last())
                                {
                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: buttonsWithSubscription, parseMode: ParseMode.Html);
                                    await changeDatabase();
                                    return;
                                }
                                else if (item == events.Last())
                                {
                                    await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                                    await changeDatabase();
                                    await paginationService.ClearPaginationListCache();
                                    return;
                                }
                                await _botClient.SendTextMessageAsync(id, response, replyMarkup: inlineKeyboardSubscription, parseMode: ParseMode.Html);
                                await changeDatabase();
                            }
                            i++;
                        }
                    }
                    return;
                }
                else
                {
                    await _botClient.SendTextMessageAsync(id, "Не має такої події, яка містила б введений вами текст!");
                    await changeDatabase();
                    return;
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                await changeDatabase();
                return;
            }

        }
        public async Task changeDatabase()
        {
            if (_context.TelegramEvent.Any())
            {
                EventActionActive TelActionEvent;
                TelActionEvent = _context.TelegramEvent.SingleOrDefault(x => x == _context.TelegramEvent.FirstOrDefault());
                _context.TelegramEvent.Remove(TelActionEvent);
                await _context.SaveChangesAsync();
            }

        }
        public string GetEventFromText(Update update, IList<DAL.Models.Event> EventList)
        {
            string text = string.Empty;
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
            return text;
        }
    }
}
