using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Notifications.DAL.Models;
using Notifications.BL.IRepository;
using Notifications.BL.Services;
namespace Notifications.BL.Commands
{
    public class GetSubscriptionCategory : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetSubscriptionCategory(IUnitOfWork unitOfWork, TelegramBot telegramBot, NotificationsContext context, IUserService userService, NotificationsService notificationsService)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
        }
        public override string Name => CommandNames.GetSubscriptionForCategory;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var data = update.CallbackQuery.Data;
            var InlineKeyboardText = returnText(update.CallbackQuery.Message.Text);
            List<bool> check = new List<bool>();

            var CategoryList = await unitOfWork.Categories.GetAll();
            var eventCategories = await unitOfWork.EventCategories.GetAll();
            if (CategoryList.Any())
            {
                foreach (var item in CategoryList)
                {
                    if (item.CategoryName == InlineKeyboardText)
                    {
                        var eventsOfCategoryList = eventCategories.Where(ec => ec.CategoryId == item.CategoryId);
                        foreach(var eventsOfCategory in eventsOfCategoryList)
                        {
                            check.Add(notificationsService.SubscriptionExists(eventsOfCategory.EventId, id.ToString()).Result);
                        }
                        if (check.Contains(false))
                        {
                            await notificationsService.SubscribeToCategory(item.CategoryId, id.ToString());
                            await _botClient.SendTextMessageAsync(id, "Ви успішно підписалися на категорію: '" + InlineKeyboardText + "'");
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(id, "Ви вже підписані на категорію: '" + InlineKeyboardText + "'");
                        }
                    }
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних подій є порожнім!");
                return;
            }
        }
        private static string returnText(string updateText)
        {
            string text = null;
            foreach (char i in updateText)
            {
                if (i == '\n')
                    return text;
                text += i;
            }
            return text;
        }
    }
}
