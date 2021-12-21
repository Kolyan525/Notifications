using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        NotificationsContext context;
        ILogger<EventController> logger;

        public EventController(NotificationsContext context, ILogger<EventController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        // GET: api/<EventController>
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await context.Events.ToListAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetEvents)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // GET api/<EventController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EventController>
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok(value);
        }

        // PUT api/<EventController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EventController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
