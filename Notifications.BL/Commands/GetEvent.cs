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
using Microsoft.AspNetCore.Mvc;
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

                foreach (var item in EventList)
                {
                    if (item.Title == update.Message.Text)
                    {
                        var response = item.Title + ".\n" + item.Description;
                        if (_context.SubscriptionEvents.Any())
                        {
                            var check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                            //var list = await unitOfWork.SubscriptionEvents.GetAll();
                            //bool check = checkFunction(list, item.EventId);
                            if (check == true)
                            {
                                await _botClient.SendTextMessageAsync(id, response, ParseMode.Markdown, replyMarkup: inlineKeyboardUnsubscribe);
                                TelActionEvent = _context.telegramEvent.SingleOrDefault(x => x == _context.telegramEvent.FirstOrDefault());
                                _context.telegramEvent.Remove(TelActionEvent);
                                await _context.SaveChangesAsync();
                                return;
                            }
                        }
                        await _botClient.SendTextMessageAsync(id, response, ParseMode.Markdown, replyMarkup: inlineKeyboardSubscription);
                        TelActionEvent = _context.telegramEvent.SingleOrDefault(x => x == _context.telegramEvent.FirstOrDefault());
                        _context.telegramEvent.Remove(TelActionEvent);
                        await _context.SaveChangesAsync();
                        return;
                    }
                }
                await _botClient.SendTextMessageAsync(id, "Не має такої події!");
                TelActionEvent = _context.telegramEvent.SingleOrDefault(x => x == _context.telegramEvent.FirstOrDefault());
                _context.telegramEvent.Remove(TelActionEvent);
                await _context.SaveChangesAsync();
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                TelActionEvent = _context.telegramEvent.SingleOrDefault(x => x == _context.telegramEvent.FirstOrDefault());
                _context.telegramEvent.Remove(TelActionEvent);
                await _context.SaveChangesAsync();
                return;
            }

        }
        //private static bool checkFunction(IList<SubscriptionEvent> list, long EventId)
        //{
        //    foreach (var i in list)
        //    {
        //        if (i.EventId == EventId)
        //            return true;
        //    }
        //    return false;
        //}
    }
}
