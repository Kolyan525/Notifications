using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using System;

namespace Notifications.BL.Services.Telegram
{
    public class TelegramBot
    {
        private readonly IConfiguration _configuration;
        private TelegramBotClient _botClient;

        public TelegramBot(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TelegramBotClient> GetBot()
        {
            if (_botClient != null)
            {
                return _botClient;
            }

            _botClient = new TelegramBotClient(_configuration["Token"]);
            //{ Timeout = TimeSpan.FromSeconds(10)};

            var hook = $"{_configuration["Url"]}api/message/update";
            await _botClient.SetWebhookAsync(hook);

            return _botClient;
        }
    }
}
