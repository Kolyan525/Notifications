using System.Threading.Tasks;
using Telegram.Bot.Types;
using Notifications.DAL.Models.Telegram;

namespace Notifications.BL.Services.Telegram
{
    public interface IUserService
    {
        Task<TelegramUser> GetOrCreate(Update update);
    }
}
