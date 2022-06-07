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
    public class GetCategories : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;

        public GetCategories(TelegramBot telegramBot, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
        }

        public override string Name => CommandNames.GetCategories;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var CategoryList = await unitOfWork.Categories.GetAll();
            var AllEventList = await unitOfWork.Events.GetAll();
            var AllEventCategoriesList = await unitOfWork.EventCategories.GetAll();
            bool lastEvent = false;
            //bool checkValue = false;
            DAL.Models.Event @event;
            string message = "Події, які відносяться до даної категорії:";
            int i = 0;

            if (CategoryList.Any())
            {
                InlineKeyboardMarkup inlineKeyboardNextEvents = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents")
                    }
                });
                InlineKeyboardMarkup inlineKeyboardPreviousEvents = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents")
                    }
                });
                InlineKeyboardMarkup buttons = new(new[]
                {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents"),
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents")
                    }
                });
                if (update.Type == UpdateType.Message)
                {
                    foreach (var category in CategoryList)
                    {
                        i = 0;
                        string eventsOfCategory = string.Empty;
                        if (AllEventCategoriesList.Any())
                        {
                            var EventCategoriesList = AllEventCategoriesList.Where(c => c.CategoryId == category.CategoryId).ToList();
                            if (EventCategoriesList.Any())
                            {
                                foreach (var eventCategory in EventCategoriesList)
                                {
                                    @event = AllEventList.FirstOrDefault(x => x.EventId == eventCategory.EventId);
                                    if (@event != null)
                                    {
                                        if(i == 0 && eventCategory != EventCategoriesList.Last())
                                        {
                                            eventsOfCategory += @event.Title + ".";
                                            break;
                                        }
                                        if (eventCategory == EventCategoriesList.Last())
                                            lastEvent = true;

                                        eventsOfCategory += @event.Title + ".\n";
                                    }
                                    i++;
                                }
                            }
                        }
                        if (eventsOfCategory == string.Empty)
                        { eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї."; lastEvent = true; }

                        if (lastEvent == true)
                        {
                            await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                                $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                                replyMarkup: null,
                                parseMode: ParseMode.Html);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                                $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                                replyMarkup: inlineKeyboardNextEvents,
                                parseMode: ParseMode.Html);
                        }
                        lastEvent = false;
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    string text = string.Empty;
                    string eventsFromCallbackQueryText = update.CallbackQuery.Message.Text;
                    string otherValue = string.Empty;
                    List<string> eventsOfCategoryList = new List<string>();
                    int k = 0;
                    bool last = false;
                    bool medium = true;
                    bool first = false;

                    foreach (var t in update.CallbackQuery.Message.Text)
                    {
                        if (t == '\n')
                        {
                            bool check = CategoryList.FirstOrDefault(x => x.CategoryName == text).CategoryName == text;
                            if (check == true)
                                break;
                        }
                        text += t;
                    }

                    var currentCategory = CategoryList.FirstOrDefault(e => e.CategoryName == text);

                    if (currentCategory != null)
                    {
                        eventsFromCallbackQueryText = eventsFromCallbackQueryText.Replace(text + "\n" + message + "\n", string.Empty);

                        if (eventsFromCallbackQueryText[eventsFromCallbackQueryText.Length - 1] == '.')
                            eventsFromCallbackQueryText = removeLastChar(eventsFromCallbackQueryText);

                        string eventsOfCategory = string.Empty;

                        if (AllEventCategoriesList.Any())
                        {
                            var EventCategoriesList = AllEventCategoriesList.Where(c => c.CategoryId == currentCategory.CategoryId).ToList();

                            if (EventCategoriesList.Any())
                            {
                                if (update.CallbackQuery.Data.Contains("NextCategoryEvents"))
                                {
                                    k = 0;
                                    for (int currentEventCategory = 0; currentEventCategory < EventCategoriesList.Count; currentEventCategory++)
                                    {
                                        if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                                        {
                                            for (int item = 0; item < eventsFromCallbackQueryText.Length; item++)
                                            {
                                                otherValue += eventsFromCallbackQueryText[item];
                                                
                                                bool ch = AllEventList.Where(x => x.Title == otherValue).Any();
                                                if (ch)
                                                {
                                                    if (item == eventsFromCallbackQueryText.Length - 1)
                                                        break;

                                                    if ((item + 1) != eventsFromCallbackQueryText.Length - 2 && (item + 2) != eventsFromCallbackQueryText.Length - 1)
                                                        if (eventsFromCallbackQueryText[item + 1] == '.' && eventsFromCallbackQueryText[item + 2] == '\n')
                                                            eventsFromCallbackQueryText = eventsFromCallbackQueryText.Remove(item + 1, 2);
                                                    
                                                    otherValue = string.Empty;
                                                }
                                            }
                                        }

                                        @event = AllEventList.FirstOrDefault(x => x.EventId == EventCategoriesList[currentEventCategory].EventId);

                                        if (@event != null && k > 0)
                                        {
                                            if (i == 0)
                                            {
                                                if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                                                {
                                                    if (first != true)
                                                        last = true; medium = false;
                                                }

                                                eventsOfCategory += @event.Title + ".";
                                                break;
                                            }

                                            if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                                            { first = true; medium = false; }

                                            if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                                            {
                                                if (first != true)
                                                    last = true; medium = false;
                                            }

                                            eventsOfCategory += @event.Title + ".\n";
                                            i++;
                                        }
                                        if (@event.Title == otherValue)
                                        {
                                            k++;
                                        }
                                    }
                                }
                                else if (update.CallbackQuery.Data.Contains("PreviousCategoryEvents"))
                                {
                                    k = 0;
                                    for (var currentEventCategory = EventCategoriesList.Count - 1; currentEventCategory >= 0; currentEventCategory--)
                                    {
                                        if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                                        {
                                            for (int item = 0; item < eventsFromCallbackQueryText.Length; item++)
                                            {
                                                otherValue += eventsFromCallbackQueryText[item];
                                                bool ch = AllEventList.Where(x => x.Title == otherValue).Any();

                                                if (ch)
                                                    break;
                                            }
                                        }

                                        @event = AllEventList.FirstOrDefault(x => x.EventId == EventCategoriesList[currentEventCategory].EventId);    

                                        if (@event != null && k > 0)
                                        {
                                            if (i == 0)
                                            {
                                                if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                                                {
                                                    if (first != true)
                                                        last = true; medium = false;
                                                }
                                                if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                                                { first = true; medium = false; }

                                                eventsOfCategoryList.Add(@event.Title + ".");
                                                break;
                                            }

                                            if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                                            { first = true; medium = false; }

                                            if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                                            { 
                                                if(first != true)
                                                    last = true; medium = false; 
                                            }

                                            eventsOfCategoryList.Add(@event.Title + ".");
                                            i++;
                                        }
                                        if (@event.Title == otherValue)
                                        {
                                            k++;
                                        }
                                    }
                                }
                            }
                        }

                        if (last == true)
                        {
                            if (eventsOfCategory == string.Empty)
                                eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї.";

                            await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{currentCategory.CategoryName}</b></u>" +
                                $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                                replyMarkup: inlineKeyboardPreviousEvents,
                                parseMode: ParseMode.Html);
                        }
                        else if (first == true)
                        {
                            if (eventsOfCategoryList.Count != 0)
                            {
                                for (int element = eventsOfCategoryList.Count - 1; element >= 0; element--)
                                {
                                    if (element == 0)
                                    { eventsOfCategory += eventsOfCategoryList[element]; break; }

                                    eventsOfCategory += eventsOfCategoryList[element] + "\n";
                                }
                            }
                            if (eventsOfCategory == string.Empty)
                                eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї.";

                            await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{currentCategory.CategoryName}</b></u>" +
                                $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                                replyMarkup: inlineKeyboardNextEvents,
                                parseMode: ParseMode.Html);
                        }
                        else if (medium == true)
                        {
                            if (eventsOfCategoryList.Count != 0)
                            {
                                for (int element = eventsOfCategoryList.Count - 1; element >= 0; element--)
                                {
                                    if (element == 0)
                                    { eventsOfCategory += eventsOfCategoryList[element]; break; }

                                    eventsOfCategory += eventsOfCategoryList[element] + "\n";
                                }
                            }
                            if (eventsOfCategory == string.Empty)
                                eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї.";

                            await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{currentCategory.CategoryName}</b></u>" +
                                $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                                replyMarkup: buttons,
                                parseMode: ParseMode.Html);
                        }
                    }
                }
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх категорій є порожнім!");
                return;
            }

        }
        public string reverseText (string text)
        {
            string reverseText = string.Empty;
            var newText = text.Reverse();
            foreach (var item in newText)
            {
                reverseText += item;
            }
            return reverseText;
        }
        public string removeLastChar(string text)
        {
            string newText = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (i == text.Length - 1)
                    if (text[i] == '.')
                        break;
                newText += text[i];
            }
            return newText;
        }
    }
}
