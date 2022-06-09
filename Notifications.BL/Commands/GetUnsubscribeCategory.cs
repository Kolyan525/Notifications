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
    public class GetUnsubscribeCategory : BaseCommand
    {
        private readonly TelegramBotClient _botClient;
        private readonly NotificationsContext _context;
        private readonly IUserService _userService;
        readonly IUnitOfWork unitOfWork;
        readonly NotificationsService notificationsService;

        public GetUnsubscribeCategory(IUnitOfWork unitOfWork, TelegramBot telegramBot, NotificationsContext context, IUserService userService, NotificationsService notificationsService)
        {
            _context = context;
            _userService = userService;
            _botClient = telegramBot.GetBot().Result;
            this.unitOfWork = unitOfWork;
            this.notificationsService = notificationsService;
        }
        public override string Name => CommandNames.GetUnsubscribeFromCategory;

        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userService.GetOrCreate(update);
            var id = user.ChatId;
            var data = update.CallbackQuery.Data;
            var InlineKeyboardText = returnText(update.CallbackQuery.Message.Text);
            List<bool> check = new List<bool>();

            var Categories = await unitOfWork.Categories.GetAll();
            var eventCategories = await unitOfWork.EventCategories.GetAll();
            if (Categories.Any())
            {
                var category = Categories.FirstOrDefault(e => e.CategoryName == InlineKeyboardText);
                var eventsOfCategoryList = eventCategories.Where(ec => ec.CategoryId == category.CategoryId);
                foreach (var eventsOfCategory in eventsOfCategoryList)
                {
                    check.Add(notificationsService.SubscriptionExists(eventsOfCategory.EventId, id.ToString()).Result);
                }
                if (!check.Contains(false))
                {
                    await notificationsService.UnsubscribeFromCategory(category.CategoryId, id.ToString());
                    await _botClient.SendTextMessageAsync(id, "Ви відписалися від категорії: '" + InlineKeyboardText + "'");
                    return;
                }
                else
                {
                    await _botClient.SendTextMessageAsync(id, "Для того, щоб відписатися від категорії '" + InlineKeyboardText + "' спочатку підпишіться на неї");
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(id, "Вибачте, але на даний момент список всіх наявних категорій є порожнім!");
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
