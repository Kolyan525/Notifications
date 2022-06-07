using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Notifications.BL.Services.Telegram;

namespace Notifications.Api.Controllers
{
    [ApiController]
    [Route("api/message/update")]
    public class TelegramBotController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;

        public TelegramBotController(ICommandExecutor _commandExecutor)
        {
            commandExecutor = _commandExecutor;
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
                await commandExecutor.Execute(upd);
            }
            catch (Exception)
            {
                return Ok();
            }

            return Ok();
        }
    }
}
