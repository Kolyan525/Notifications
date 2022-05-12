using System.Threading.Tasks;
using Telegram.Bot.Types;


namespace Notifications.BL.Services.Telegram
{
    public interface ICommandExecutor
    {
        Task Execute(Update update);
    }
}
