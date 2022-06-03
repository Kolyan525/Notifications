using Notifications.BL.IRepository;
using Notifications.BL.Services.Telegram;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            var id = user.ChatId;
            var EventList = await unitOfWork.Events.GetAll();
            if (EventList.Any())
            {
                InlineKeyboardMarkup inlineKeyboardDetail = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    }
                });
                InlineKeyboardMarkup buttons = new(new[]
                {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextEvents")
                    }
                });
                int i = 0;
                if (update.Type == UpdateType.Message)
                {
                    foreach (var Ev in EventList)
                    {
                        if (i == 1 && Ev != EventList.Last())
                        {
                            await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                            replyMarkup: buttons, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                            return;
                        }
                        await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                            replyMarkup: inlineKeyboardDetail, parseMode: ParseMode.Html);
                        i++;
                    }
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    string text = "";
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
                    var lastEvent = EventList.FirstOrDefault(e => e.Title == text);
                    List<DAL.Models.Event> NewEventList = new List<DAL.Models.Event>();
                    int k = 0;
                    foreach (var e in EventList)
                    {
                        if (k > 0)
                        {
                            NewEventList.Add(e);
                        }
                        if (e == lastEvent)
                            k++;
                    }
                    if (NewEventList.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        foreach (var Ev in NewEventList)
                        {
                            if (i == 1 && Ev != NewEventList.Last())
                            {
                                await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                                replyMarkup: buttons, parseMode: ParseMode.Html);
                                return;
                            }
                            await _botClient.SendTextMessageAsync(id, $"<u><b>{Ev.Title}</b></u>\n{Ev.ShortDesc}.\n",
                                replyMarkup: inlineKeyboardDetail, parseMode: ParseMode.Html);
                            i++;
                        }
                    }
                }
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
