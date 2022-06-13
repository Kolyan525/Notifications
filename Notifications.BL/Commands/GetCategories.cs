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
        readonly NotificationsService notificationsService;
        List<bool> check = new List<bool>();
        bool lastCategory = false;
        bool lastEvent = false;
        bool lastButtonsWithoutNextCategories = false;
        bool mediumButtonsWithoutNextCategories = true;
        bool firstButtonsWithoutNextCategories = false;
        bool lastButtonsWithNextCategories = false;
        bool mediumButtonsWithNextCategories = true;
        bool firstButtonsWithNextCategories = false;
        int i = 0, j = 0; //t = 0;
        InlineKeyboardMarkup inlineKeyboardSubscription, inlineKeyboardUnsubscribe, inlineKeyboardNextEventsWithSubscription,
            inlineKeyboardNextEventsWithUnsubscribe, inlineKeyboardPreviousEventsWithSubscription, inlineKeyboardPreviousEventsWithUnsubscribe,
            buttonsWithSubscription, buttonsWithUnsubscribe, nextCategories, nextCategoriesWithSubscription, nextCategoriesWithUnsubscribe,
            nextCategoriesWithNextEventsWithSubscription, nextCategoriesWithNextEventsWithUnsubscribe, nextCategoriesWithPreviousEventsWithSubscription,
            nextCategoriesWithPreviousEventsWithUnsubscribe, nextCategoriesWithButtonsWithSubscription, nextCategoriesWithButtonsWithUnsubscribe;

        public GetCategories(TelegramBot telegramBot, IUserService userService, IUnitOfWork unitOfWork, NotificationsService notificationsService)
        {
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
            inlineKeyboardSubscription = TelegramButtons.GetCategories.Subscription;
            inlineKeyboardUnsubscribe = TelegramButtons.GetCategories.Unsubscribe;
            inlineKeyboardNextEventsWithSubscription = TelegramButtons.GetCategories.NextEventsWithSubscription;
            inlineKeyboardNextEventsWithUnsubscribe = TelegramButtons.GetCategories.NextEventsWithUnsubscribe;
            inlineKeyboardPreviousEventsWithSubscription = TelegramButtons.GetCategories.PreviousEventsWithSubscription;
            inlineKeyboardPreviousEventsWithUnsubscribe = TelegramButtons.GetCategories.PreviousEventsWithUnsubscribe;
            buttonsWithSubscription = TelegramButtons.GetCategories.ButtonsWithSubscription;
            buttonsWithUnsubscribe = TelegramButtons.GetCategories.ButtonsWithUnsubscribe;
            nextCategories = TelegramButtons.GetCategories.NextCategories;
            nextCategoriesWithSubscription = TelegramButtons.GetCategories.NextCategoriesWithSubscription;
            nextCategoriesWithUnsubscribe = TelegramButtons.GetCategories.NextCategoriesWithUnsubscribe;
            nextCategoriesWithNextEventsWithSubscription = TelegramButtons.GetCategories.NextCategoriesWithNextEventsWithSubscription;
            nextCategoriesWithNextEventsWithUnsubscribe = TelegramButtons.GetCategories.NextCategoriesWithNextEventsWithUnsubscribe;
            nextCategoriesWithPreviousEventsWithSubscription = TelegramButtons.GetCategories.NextCategoriesWithPreviousEventsWithSubscription;
            nextCategoriesWithPreviousEventsWithUnsubscribe = TelegramButtons.GetCategories.NextCategoriesWithPreviousEventsWithUnsubscribe;
            nextCategoriesWithButtonsWithSubscription = TelegramButtons.GetCategories.NextCategoriesWithButtonsWithSubscription;
            nextCategoriesWithButtonsWithUnsubscribe = TelegramButtons.GetCategories.NextCategoriesWithButtonsWithUnsubscribe;
        }

        public override string Name => CommandNames.GetCategories;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var CategoryList = await unitOfWork.Categories.GetAll();
            var AllEventList = await unitOfWork.Events.GetAll();
            var AllEventCategoriesList = await unitOfWork.EventCategories.GetAll();
            DAL.Models.Event @event = null;
            string message = "Події, які відносяться до даної категорії:";
            var eventCategories = await unitOfWork.EventCategories.GetAll();
            string eventsOfCategory = string.Empty;

            if (CategoryList.Any())
            {
                if (update.Type == UpdateType.Message)
                {
                    foreach (var category in CategoryList)
                    {
                        await OutputCtegories(category, CategoryList, eventsOfCategory, AllEventCategoriesList, id, @event, AllEventList, message);
                        if (lastCategory == true)
                            return;
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    string text = string.Empty;
                    string eventsFromCallbackQueryText = update.CallbackQuery.Message.Text;
                    string eventTitleForCheck = string.Empty;
                    List<string> eventsOfCategoryList = new List<string>();
                    int k = 0;

                    foreach (var t in update.CallbackQuery.Message.Text)
                    {
                        if (t == '\n')
                        {
                            bool chek = CategoryList.FirstOrDefault(x => x.CategoryName == text).CategoryName == text;
                            if (chek == true)
                                break;
                        }
                        text += t;
                    }

                    var currentCategory = CategoryList.FirstOrDefault(e => e.CategoryName == text);

                    if (currentCategory != null)
                    {
                        if (update.CallbackQuery.Data == "NextCategories")
                        {
                            k = 0;
                            foreach (var category in CategoryList)
                            {
                                if (k > 0)
                                {
                                    await OutputCtegories(category, CategoryList, eventsOfCategory, AllEventCategoriesList, id, @event, AllEventList, message);
                                    if (lastCategory == true)
                                        return;
                                }
                                if (category.CategoryName == currentCategory.CategoryName)
                                    k++;
                            }
                            return;
                        }

                        var eventsOfCurrentCategoryList = eventCategories.Where(ec => ec.CategoryId == currentCategory.CategoryId);
                        foreach (var thisEventsOfCategory in eventsOfCurrentCategoryList)
                        {
                            check.Add(notificationsService.SubscriptionExists(thisEventsOfCategory.EventId, id.ToString()).Result);
                        }

                        eventsFromCallbackQueryText = eventsFromCallbackQueryText.Replace(text + "\n" + message + "\n", string.Empty);

                        if (eventsFromCallbackQueryText[eventsFromCallbackQueryText.Length - 1] == '.')
                            eventsFromCallbackQueryText = removeLastChar(eventsFromCallbackQueryText);

                        eventsOfCategory = string.Empty;

                        if (AllEventCategoriesList.Any())
                        {
                            var EventCategoriesList = AllEventCategoriesList.Where(c => c.CategoryId == currentCategory.CategoryId).ToList();

                            if (EventCategoriesList.Any())
                            {
                                if (update.CallbackQuery.Data == "NextCategoryEvents")
                                {
                                    eventsOfCategory = NextCategoryEvents(update.CallbackQuery.Data, k, EventCategoriesList, eventsFromCallbackQueryText,
                                        eventTitleForCheck, AllEventList, @event, eventsOfCategory);
                                }
                                else if (update.CallbackQuery.Data == "PreviousCategoryEvents")
                                {
                                    eventsOfCategoryList = PreviousCategoryEvents(update.CallbackQuery.Data, k, EventCategoriesList, eventsFromCallbackQueryText,
                                        eventTitleForCheck, AllEventList, @event, eventsOfCategoryList);
                                }

                                else if (update.CallbackQuery.Data == "NextCategoryEventsWithNextCategories") 
                                {
                                    eventsOfCategory = NextCategoryEvents(update.CallbackQuery.Data, k, EventCategoriesList, eventsFromCallbackQueryText,
                                        eventTitleForCheck, AllEventList, @event, eventsOfCategory);
                                }
                                else if (update.CallbackQuery.Data == "PreviousCategoryEventsWithNextCategories")
                                {
                                    eventsOfCategoryList = PreviousCategoryEvents(update.CallbackQuery.Data, k, EventCategoriesList, eventsFromCallbackQueryText,
                                        eventTitleForCheck, AllEventList, @event, eventsOfCategoryList);
                                }
                            }
                        }

                        if (lastButtonsWithoutNextCategories == true)
                        {
                            if (eventsOfCategory == string.Empty)
                                eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї.";

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
                        }


                        else if (firstButtonsWithoutNextCategories == true)
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

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
                        }


                        else if (mediumButtonsWithoutNextCategories == true)
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

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
                        }


                        else if (lastButtonsWithNextCategories == true)
                        {
                            if (eventsOfCategory == string.Empty)
                                eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї.";

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
                        }


                        else if (firstButtonsWithNextCategories == true)
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

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
                        }


                        else if (mediumButtonsWithNextCategories == true)
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

                            await OutputCategoryWithButtons(eventsOfCategory, id, update, currentCategory);
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
        public async Task OutputCtegories(Category category, IList<Category> CategoryList, string eventsOfCategory, IList<EventCategory> AllEventCategoriesList, long id,
            DAL.Models.Event @event, IList<DAL.Models.Event> AllEventList, string message)
        {
            if (j == 4 && category != CategoryList.Last())
            {
                i = 0;
                eventsOfCategory = string.Empty;
                if (AllEventCategoriesList.Any())
                {
                    eventsOfCategory = EventsOfCategory(AllEventCategoriesList, category, id, @event, AllEventList, eventsOfCategory);
                }
                if (eventsOfCategory == string.Empty)
                { eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї."; /*lastEvent = true;*/ }

                if (lastEvent == true)
                {
                    if (eventsOfCategory == "В даної категорії наразі немає подій, які відносяться до неї.")
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: nextCategories,
                            parseMode: ParseMode.Html);
                    }
                    else if (check.Contains(false) && check.Any())
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: nextCategoriesWithSubscription,
                            parseMode: ParseMode.Html);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                           $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                           replyMarkup: nextCategoriesWithUnsubscribe,
                           parseMode: ParseMode.Html);
                    }
                }
                else
                {
                    if (eventsOfCategory == "В даної категорії наразі немає подій, які відносяться до неї.")
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: nextCategories,
                            parseMode: ParseMode.Html);
                    }
                    else if (check.Contains(false) && check.Any())
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: nextCategoriesWithNextEventsWithSubscription,
                            parseMode: ParseMode.Html);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: nextCategoriesWithNextEventsWithUnsubscribe,
                            parseMode: ParseMode.Html);
                    }
                }
                lastEvent = false;
                check.Clear();
                lastCategory = true;
                return;
            }

            else
            {
                i = 0;
                eventsOfCategory = string.Empty;
                if (AllEventCategoriesList.Any())
                {
                    eventsOfCategory = EventsOfCategory(AllEventCategoriesList, category, id, @event, AllEventList, eventsOfCategory);
                }
                if (eventsOfCategory == string.Empty)
                { eventsOfCategory = "В даної категорії наразі немає подій, які відносяться до неї."; /*lastEvent = true;*/ }

                if (lastEvent == true)
                {
                    if (eventsOfCategory == "В даної категорії наразі немає подій, які відносяться до неї.")
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: null,
                            parseMode: ParseMode.Html);
                    }
                    else if (check.Contains(false) && check.Any())
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: inlineKeyboardSubscription,
                            parseMode: ParseMode.Html);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                           $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                           replyMarkup: inlineKeyboardUnsubscribe,
                           parseMode: ParseMode.Html);
                    }
                }
                else
                {
                    if (eventsOfCategory == "В даної категорії наразі немає подій, які відносяться до неї.")
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: null,
                            parseMode: ParseMode.Html);
                    }
                    else if (check.Contains(false) && check.Any())
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: inlineKeyboardNextEventsWithSubscription,
                            parseMode: ParseMode.Html);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{category.CategoryName}</b></u>" +
                            $"\n<b>{message}</b>\n{eventsOfCategory}\n",
                            replyMarkup: inlineKeyboardNextEventsWithUnsubscribe,
                            parseMode: ParseMode.Html);
                    }
                }
                lastEvent = false;
                check.Clear();
                j++;
            }
        }

        public async Task OutputCategoryWithButtons(string eventsOfCategory, long id, Update update, Category category)
        {
            InlineKeyboardMarkup buttons1 = null;
            InlineKeyboardMarkup buttons2 = null;
            InlineKeyboardMarkup buttons3 = null;
            bool nullButtonsActive = false;

            if (lastButtonsWithoutNextCategories == true)
            {
                nullButtonsActive = true;
                buttons2 = inlineKeyboardPreviousEventsWithSubscription;
                buttons3 = inlineKeyboardPreviousEventsWithUnsubscribe;
            }
            else if (firstButtonsWithoutNextCategories == true)
            {
                nullButtonsActive = true;
                buttons2 = inlineKeyboardNextEventsWithSubscription;
                buttons3 = inlineKeyboardNextEventsWithUnsubscribe;
            }
            else if (mediumButtonsWithoutNextCategories == true)
            {
                nullButtonsActive = true;
                buttons2 = buttonsWithSubscription;
                buttons3 = buttonsWithUnsubscribe;
            }
            else if (lastButtonsWithNextCategories == true)
            {
                buttons1 = nextCategories;
                buttons2 = nextCategoriesWithPreviousEventsWithSubscription;
                buttons3 = nextCategoriesWithPreviousEventsWithUnsubscribe;
            }
            else if (firstButtonsWithNextCategories == true)
            {
                buttons1 = nextCategories;
                buttons2 = nextCategoriesWithNextEventsWithSubscription;
                buttons3 = nextCategoriesWithNextEventsWithUnsubscribe;
            }
            else if (mediumButtonsWithNextCategories == true)
            {
                buttons1 = nextCategories;
                buttons2 = nextCategoriesWithButtonsWithSubscription;
                buttons3 = nextCategoriesWithButtonsWithUnsubscribe;
            }

            if (eventsOfCategory == "В даної категорії наразі немає подій, які відносяться до неї.")
            {
                if (nullButtonsActive == true)
                {
                    await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{category.CategoryName}</b></u>" +
                         $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                         replyMarkup: null,
                         parseMode: ParseMode.Html);
                }
                else
                {
                    await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{category.CategoryName}</b></u>" +
                         $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                         replyMarkup: buttons1,
                         parseMode: ParseMode.Html);
                }
            }
            else if (check.Contains(false) && check.Any())
            {
                await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{category.CategoryName}</b></u>" +
                    $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                    replyMarkup: buttons2,
                    parseMode: ParseMode.Html);
            }
            else
            {
                await _botClient.EditMessageTextAsync(id, update.CallbackQuery.Message.MessageId, $"<u><b>{category.CategoryName}</b></u>" +
                    $"\n<b>Події, які відносяться до даної категорії:</b>\n{eventsOfCategory}\n",
                    replyMarkup: buttons3,
                    parseMode: ParseMode.Html);
            }
        }
        private bool checkEvent(IList<DAL.Models.Event> EventList, string text)
        {
            //if (text.First() == '\n')
            //    text = text.Remove(0);
            foreach (var item in EventList)
            {
                if (item.Title == text)
                    return true;
            }
            return false;
        }
        public string NextCategoryEvents(string updateData, int k, List<EventCategory> EventCategoriesList, string eventsFromCallbackQueryText, string eventTitleForCheck,
            IList<DAL.Models.Event> AllEventList, DAL.Models.Event @event, string eventsOfCategory)
        {
            bool lastButtons = false;
            bool mediumButtons = true;
            bool firstButtons = false;
            bool otherMediumButtons = false;
            k = 0;
            for (int currentEventCategory = 0; currentEventCategory < EventCategoriesList.Count; currentEventCategory++)
            {
                if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                {
                    for (int item = 0; item < eventsFromCallbackQueryText.Length; item++)
                    {
                        eventTitleForCheck += eventsFromCallbackQueryText[item];

                        if (eventTitleForCheck.First() == '\n')
                            eventTitleForCheck = eventTitleForCheck.Remove(0);
                        //bool ch = AllEventList.Where(x => x.Title == eventTitleForCheck).Any();
                        bool ch = checkEvent(AllEventList, eventTitleForCheck);
                        if (ch)
                        {
                            if (item == eventsFromCallbackQueryText.Length - 1)
                                break;

                            if ((item + 1) != eventsFromCallbackQueryText.Length - 2 && (item + 2) != eventsFromCallbackQueryText.Length - 1)
                                if (eventsFromCallbackQueryText[item + 1] == '.' && eventsFromCallbackQueryText[item + 2] == '\n')
                                    eventsFromCallbackQueryText = eventsFromCallbackQueryText.Remove(item + 1, 2);

                            eventTitleForCheck = string.Empty;
                        }
                    }
                }

                @event = AllEventList.FirstOrDefault(x => x.EventId == EventCategoriesList[currentEventCategory].EventId);

                if (@event != null && k > 0)
                {
                    if (i == 2)
                    {
                        if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                        {
                            if (firstButtons != true)
                                lastButtons = true; mediumButtons = false;
                        }

                        eventsOfCategory += @event.Title;
                        break;
                    }

                    if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                    { firstButtons = true; mediumButtons = false; }

                    if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                    {
                        if (firstButtons != true)
                            lastButtons = true; mediumButtons = false;
                    }

                    eventsOfCategory += @event.Title + "\n";
                    i++;
                }
                if (@event.Title == eventTitleForCheck)
                {
                    k++;
                }
            }
            if (updateData == "NextCategoryEvents")
            { 
                firstButtonsWithoutNextCategories = firstButtons;
                mediumButtonsWithoutNextCategories = mediumButtons;
                lastButtonsWithoutNextCategories = lastButtons;
                mediumButtonsWithNextCategories = otherMediumButtons;
            }
            else if (updateData == "NextCategoryEventsWithNextCategories")
            {
                firstButtonsWithNextCategories = firstButtons;
                mediumButtonsWithNextCategories = mediumButtons;
                lastButtonsWithNextCategories = lastButtons;
                mediumButtonsWithoutNextCategories = otherMediumButtons;
            }
            return eventsOfCategory;
        }
        public List<string> PreviousCategoryEvents(string updateData, int k, List<EventCategory> EventCategoriesList, string eventsFromCallbackQueryText, string eventTitleForCheck,
            IList<DAL.Models.Event> AllEventList, DAL.Models.Event @event, List<string> eventsOfCategoryList)
        {
            bool lastButtons = false;
            bool mediumButtons = true;
            bool firstButtons = false;
            bool otherMediumButtons = false;
            k = 0;
            for (var currentEventCategory = EventCategoriesList.Count - 1; currentEventCategory >= 0; currentEventCategory--)
            {
                if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                {
                    for (int item = 0; item < eventsFromCallbackQueryText.Length; item++)
                    {
                        eventTitleForCheck += eventsFromCallbackQueryText[item];
                        bool ch = AllEventList.Where(x => x.Title == eventTitleForCheck).Any();

                        if (ch)
                            break;
                    }
                }

                @event = AllEventList.FirstOrDefault(x => x.EventId == EventCategoriesList[currentEventCategory].EventId);

                if (@event != null && k > 0)
                {
                    if (i == 2)
                    {
                        if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                        {
                            if (firstButtons != true)
                                lastButtons = true; mediumButtons = false;
                        }
                        if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                        { firstButtons = true; mediumButtons = false; }

                        eventsOfCategoryList.Add(@event.Title);
                        break;
                    }

                    if (EventCategoriesList[currentEventCategory] == EventCategoriesList.First())
                    { firstButtons = true; mediumButtons = false; }

                    if (EventCategoriesList[currentEventCategory] == EventCategoriesList.Last())
                    {
                        if (firstButtons != true)
                            lastButtons = true; mediumButtons = false;
                    }

                    eventsOfCategoryList.Add(@event.Title);
                    i++;
                }
                if (@event.Title == eventTitleForCheck)
                {
                    k++;
                }
            }
            if (updateData == "PreviousCategoryEvents")
            {
                firstButtonsWithoutNextCategories = firstButtons;
                mediumButtonsWithoutNextCategories = mediumButtons;
                lastButtonsWithoutNextCategories = lastButtons;
                mediumButtonsWithNextCategories = otherMediumButtons;
            }
            else if (updateData == "PreviousCategoryEventsWithNextCategories")
            {
                firstButtonsWithNextCategories = firstButtons;
                mediumButtonsWithNextCategories = mediumButtons;
                lastButtonsWithNextCategories = lastButtons;
                mediumButtonsWithoutNextCategories = otherMediumButtons;
            }
            return eventsOfCategoryList;
        }
        public string EventsOfCategory(IList<EventCategory> AllEventCategoriesList, Category category, 
            long id, DAL.Models.Event @event, IList<DAL.Models.Event> AllEventList, string eventsOfCategory)
        {
            var EventCategoriesList = AllEventCategoriesList.Where(c => c.CategoryId == category.CategoryId);
            if (EventCategoriesList.Any())
            {
                foreach (var eventCategory in EventCategoriesList)
                {
                    check.Add(notificationsService.SubscriptionExists(eventCategory.EventId, id.ToString()).Result);
                }
                foreach (var eventCategory in EventCategoriesList)
                {
                    @event = AllEventList.FirstOrDefault(x => x.EventId == eventCategory.EventId);
                    if (@event != null)
                    {
                        if (i == 2 && eventCategory != EventCategoriesList.Last())
                        {
                            eventsOfCategory += @event.Title;
                            break;
                        }
                        if (eventCategory == EventCategoriesList.Last())
                            lastEvent = true;

                        eventsOfCategory += @event.Title + "\n";
                    }
                    i++;
                }
            }
            return eventsOfCategory;
        }
    }
}
