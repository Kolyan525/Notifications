using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;
using Microsoft.Extensions.Logging;

namespace Notifications.Api.Controllers
{
    [ApiController]
    [Route("api/message/update")]
    public class TelegramBotController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;
        ILogger<TelegramBotController> logger;

        public TelegramBotController(ICommandExecutor _commandExecutor, ILogger<TelegramBotController> logger)
        {
            commandExecutor = _commandExecutor;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] object update)
        {
            // /start => register user

            var upd = JsonConvert.DeserializeObject<Update>(update.ToString());

            if (upd?.Message?.Chat == null && upd?.CallbackQuery == null)
            {
                return Ok();
            }

            try
            {
                logger.LogInformation($"Executing command #{upd}");
                await commandExecutor.Execute(upd);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong when executing bot command");
                return Ok();
            }

            return Ok();
        }
    }
}
