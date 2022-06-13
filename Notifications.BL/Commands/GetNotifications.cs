using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.BL.IRepository;
using Notifications.BL.Services;
using Hangfire;
using System;
using Telegram.Bot.Types.Enums;

namespace Notifications.BL.Commands
{
    public class GetNotifications : BaseCommand
    {
        readonly NotificationsContext context;
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;
        readonly ICacheService cacheService;
        bool checkEvent = false;

        public GetNotifications(IUnitOfWork unitOfWork, TelegramBot telegramBot, IUserService userService, NotificationsService notificationsService, ICacheService cacheService, NotificationsContext context)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
            this.cacheService = cacheService;
            this.context = context;
        }

        public override string Name => CommandNames.GetNotifications;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var buttons = TelegramButtons.GetSubscription.Buttons;
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
            string val = string.Empty;
            string InlineKeyboardText = string.Empty;
            DateTime now = DateTime.Now;
            if (context.TelegramEvent.Any())
            {
                foreach (var telActionEvent in context.TelegramEvent)
                {
                    context.TelegramEvent.Remove(telActionEvent);
                    await context.SaveChangesAsync();
                }
            }

            var EventList = await unitOfWork.Events.GetAll();
            if (EventList.Any())
            {
                if (update.Type == UpdateType.Message)
                {
                    InlineKeyboardText = update.Message.Text;
                    if (int.TryParse(InlineKeyboardText.Last().ToString(), out int value))
                        InlineKeyboardText = removeLastCharsIfNotInt(InlineKeyboardText);


                    for (int i = 0; i < InlineKeyboardText.Length; i++)
                    {
                        if (int.TryParse(InlineKeyboardText[i].ToString(), out int number))
                        {
                            val += InlineKeyboardText[i].ToString();

                            if (i == (InlineKeyboardText.Length - 1))
                            {
                                minute = int.Parse(val); val = string.Empty;
                                break;
                            }
                            else if (InlineKeyboardText[i + 1] == ',' || !(int.TryParse(InlineKeyboardText[i + 1].ToString(), out int num)))
                            {
                                if (val[0] == '0')
                                    val = val.Remove(0);
                                if (year > 0)
                                {
                                    if (month > 0)
                                    {
                                        if (day > 0)
                                        {
                                            if (hour > 0)
                                            {
                                                if (minute > 0)
                                                    break;
                                                else
                                                { minute = int.Parse(val); val = string.Empty; }
                                            }
                                            else
                                            { hour = int.Parse(val); val = string.Empty; }
                                        }
                                        else
                                        { day = int.Parse(val); val = string.Empty; }
                                    }
                                    else
                                    { month = int.Parse(val); val = string.Empty; }
                                }
                                else
                                { year = int.Parse(val); val = string.Empty; }
                            }
                        }
                    }
                    if (year == 0 || month == 0 || day == 0)
                    {
                        await _botClient.SendTextMessageAsync(id, "Натисніть ще раз на кнопку \"Обрати свій варіант\" і введіть дату за форматом \"рік, місяць, день, година, хвилина\"");
                        return;
                    }
                    var @event = cacheService.GetEventFromCache().Result;
                    if (@event != null)
                    {
                        var time = new DateTime(year, month, day, hour, minute, second);

                        if
                            (
                            time.Year > @event.StartAt.Year ||
                            time.Month > @event.StartAt.Month && time.Year == @event.StartAt.Year ||
                            time.Day > @event.StartAt.Day && time.Year == @event.StartAt.Year && time.Month == @event.StartAt.Month ||
                            time.Hour > @event.StartAt.Hour && time.Year == @event.StartAt.Year && time.Month == @event.StartAt.Month && time.Day == @event.StartAt.Day ||
                            time.Minute > @event.StartAt.Minute && time.Year == @event.StartAt.Year && time.Month == @event.StartAt.Month && time.Day == @event.StartAt.Day && time.Hour == @event.StartAt.Hour ||
                            time.Second > @event.StartAt.Second && time.Year == @event.StartAt.Year && time.Month == @event.StartAt.Month && time.Day == @event.StartAt.Day && time.Hour == @event.StartAt.Hour && time.Minute == @event.StartAt.Minute ||
                            time.Second == @event.StartAt.Second && time.Year == @event.StartAt.Year && time.Month == @event.StartAt.Month && time.Day == @event.StartAt.Day && time.Hour == @event.StartAt.Hour && time.Minute == @event.StartAt.Minute
                            )
                        {
                            await _botClient.SendTextMessageAsync(id, "Ви не можете вказати дату пізніше за дату початку події!");
                            await cacheService.ClearActiveCacheEventCache();
                            await cacheService.ClearEventFromCache();
                            return;
                        }

                        var timeToEvent = @event.StartAt.ToLocalTime().Subtract(time);

                        if (now > time)
                        {
                            await _botClient.SendTextMessageAsync(id, "Вкажіть інший варіант отримання нагадувань, так як дана подія розпочнеться раніше за вказаний вами термін!");
                            await cacheService.ClearActiveCacheEventCache();
                            await cacheService.ClearEventFromCache();
                            return;
                        }

                        BackgroundJob.Schedule(
                            () => notificationsService.NotifyUser(@event, id.ToString()),
                            time);
                        //string days = "днів";
                        //string hours = "годин";
                        //string minutes = "хвилин";
                        //string seconds = "секунд";

                        //if (timeToEvent.Days == 1)
                        //    days = "день";
                        //else if(timeToEvent.Days == 2 || timeToEvent.Days == 3 || timeToEvent.Days == 4)
                        //    days = "дні";

                        //if (timeToEvent.Hours == 1)
                        //    hours = "годину";
                        //else if (timeToEvent.Hours == 2 || timeToEvent.Hours == 3 || timeToEvent.Hours == 4)
                        //    hours = "години";

                        //if (timeToEvent.Minutes == 1)
                        //    minutes = "хвилину";
                        //else if (timeToEvent.Minutes == 2 || timeToEvent.Minutes == 3 || timeToEvent.Minutes == 4)
                        //    minutes = "хвилини";

                        //if (timeToEvent.Seconds == 1)
                        //    seconds = "секунду";
                        //else if (timeToEvent.Seconds == 2 || timeToEvent.Seconds == 3 || timeToEvent.Seconds == 4)
                        //    seconds = "секунди";

                        //await _botClient.SendTextMessageAsync(id, $"Чудово! Тепер ви отримаєте нагадування про подію \"{@event.Title}\" за {timeToEvent.Days} {days}," +
                        //    $" {timeToEvent.Hours} {hours}, {timeToEvent.Minutes} {minutes}, {timeToEvent.Seconds} {seconds} до її початку!");
                        await _botClient.SendTextMessageAsync(id, $"Чудово! Тепер ви отримаєте нагадування про подію \"{@event.Title}\" {time}");
                        await cacheService.ClearActiveCacheEventCache();
                        await cacheService.ClearEventFromCache();
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(id, "Упс, щось пішло не так!");
                        return;
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    var data = update.CallbackQuery.Data;
                    InlineKeyboardText = returnText(update.CallbackQuery.Message.Text, EventList);

                    if (InlineKeyboardText != null && checkEvent == true)
                    {
                        foreach (var item in EventList)
                        {
                            if (item.Title == InlineKeyboardText)
                            {
                                var check = notificationsService.SubscriptionExists(item.EventId, id.ToString()).Result;
                                if (check == true)
                                {
                                    switch (data)
                                    {
                                        case "Week":
                                            {
                                                var timeToEvent = item.StartAt.ToLocalTime().Subtract(TimeSpan.FromDays(7));

                                                if (now > timeToEvent)
                                                {
                                                    await _botClient.SendTextMessageAsync(id, "Виберіть інший варіант отримання нагадувань, так як дана подія розпочнеться раніше за вказаний вами термін!");
                                                    return;
                                                }

                                                BackgroundJob.Schedule(
                                                    () => notificationsService.NotifyUser(item, id.ToString()),
                                                    timeToEvent);

                                                await _botClient.SendTextMessageAsync(id, $"Чудово! Тепер ви отримаєте нагадування про подію за тиждень ({timeToEvent} днів) до її початку!");
                                                return;
                                            }
                                        case "Day":
                                            {
                                                var timeToEvent = item.StartAt.ToLocalTime().Subtract(TimeSpan.FromDays(1));

                                                if (now > timeToEvent)
                                                {
                                                    await _botClient.SendTextMessageAsync(id, "Виберіть інший варіант отримання нагадувань, так як дана подія розпочнеться раніше за вказаний вами термін!");
                                                    return;
                                                }

                                                BackgroundJob.Schedule(
                                                    () => notificationsService.NotifyUser(item, id.ToString()),
                                                    timeToEvent);

                                                await _botClient.SendTextMessageAsync(id, $"Чудово! Тепер ви отримаєте нагадування про подію за день \"{timeToEvent}\" до її початку!");
                                                return;
                                            }
                                        case "Hour":
                                            {
                                                var timeToEvent = item.StartAt.ToLocalTime().Subtract(TimeSpan.FromHours(1));

                                                if (now > timeToEvent)
                                                {
                                                    await _botClient.SendTextMessageAsync(id, "Виберіть інший варіант отримання нагадувань, так як дана подія розпочнеться раніше за вказаний вами термін!");
                                                    return;
                                                }

                                                BackgroundJob.Schedule(
                                                    () => notificationsService.NotifyUser(item, id.ToString()),
                                                    timeToEvent);

                                                await _botClient.SendTextMessageAsync(id, $"Чудово! Тепер ви отримаєте нагадування про подію за годину \"{timeToEvent}\" до її початку!");
                                                return;
                                            }
                                        case "Other":
                                            {
                                                await _botClient.SendTextMessageAsync(id, "Вкажіть час у числовому форматі \"рік, місяць, день, година, хвилина\", коли ви хотіли б отримувати сповіщення про початок події \"" + item.Title + "\"");
                                                cacheService.ActiveCacheEvent(item.EventId.ToString() + "Активовано");
                                                cacheService.SetEventToCache(item.EventId.ToString() + "Кеш", item);
                                                return;
                                            }
                                    }
                                }
                                else
                                {
                                    await _botClient.SendTextMessageAsync(id, "Ви ще не підписалися на подію!");
                                }
                            }
                        }
                    }
                }
                else
                {

                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                return;
            }
        }
        public string removeLastCharsIfNotInt(string text)
        {
            string newText = text;
            for (int i = newText.Length - 1; i >= 0; i++)
            {
                if (int.TryParse(newText[i].ToString(), out int value))
                    break;
                newText = newText.Remove(i);
            }
            return newText;
        }
        private string returnText(string updateText, IList<DAL.Models.Event> EventList)
        {
            string text = null;
            int k = 0;
            foreach (char i in updateText)
            {
                if (k > 0)
                {
                    text += i;
                    bool chek = check(EventList, text);
                    if (chek == true)
                    {
                        checkEvent = true;
                        break;
                    }
                }
                if (i == '\"')
                    k++;
            }
            return text;
        }
        private bool check(IList<DAL.Models.Event> EventList, string text)
        {
            foreach (var item in EventList)
            {
                if (item.Title == text)
                    return true;
            }
            return false;
        }
    }
}
