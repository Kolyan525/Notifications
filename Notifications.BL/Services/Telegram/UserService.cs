using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Notifications.DAL.Models.Telegram;
using Notifications.DAL.Models;

namespace Notifications.BL.Services.Telegram
{
    public class UserService : IUserService
    {
        private readonly NotificationsContext _context;

        public UserService(NotificationsContext context)
        {
            _context = context;
        }

        public async Task<TelegramUser> GetOrCreate(Update update)
        {
            var newUser = update.Type switch
            {
                UpdateType.CallbackQuery => new TelegramUser
                {
                    Username = update.CallbackQuery.From.Username,
                    ChatId = update.CallbackQuery.Message.Chat.Id,
                    FirstName = update.CallbackQuery.Message.From.FirstName,
                    LastName = update.CallbackQuery.Message.From.LastName
                },
                UpdateType.Message => new TelegramUser
                {
                    Username = update.Message.Chat.Username,
                    ChatId = update.Message.Chat.Id,
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName
                }
            };

            var user = await _context.TelegramUsers.FirstOrDefaultAsync(x => x.ChatId == newUser.ChatId);

            if (user != null) return user;

            var result = await _context.TelegramUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}
