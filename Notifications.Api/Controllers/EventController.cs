using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notifications.Api.IRepository;
using Notifications.DAL.Models;
using Notifications.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        readonly IUnitOfWork unitOfWork;
        readonly ILogger<EventController> logger;
        readonly IMapper mapper;

        public EventController(IUnitOfWork unitOfWork, ILogger<EventController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        // GET: api/<EventController>
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var events = await unitOfWork.Events.GetAll();
                var results = mapper.Map<IList<EventDTO>>(events);
                logger.LogInformation($"Successfully executed {nameof(GetEvents)}");
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetEvents)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // GET api/<EventController>/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<Event>> GetEvent(long id)
        {
            try
            {
                var evnt = await unitOfWork.Events.Get(x => x.EventId == id, new List<string> { "EventCategories" }); // TODO: f evnt
                var result = mapper.Map<EventDTO>(evnt);
                logger.LogInformation($"Successfully executed {nameof(GetEvent)}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
         
        // POST api/<EventController>
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event evnt) // TODO: DTO
        {
            await unitOfWork.Events.Insert(evnt);
            await unitOfWork.Save();

            return CreatedAtAction(nameof(PostEvent), new { id = evnt.EventId }, evnt);
        }

        // PUT api/<EventController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(long id, Event evnt)
        {
            if (id != evnt.EventId)
            {
                return BadRequest();
            }

            var evnt1 = await unitOfWork.Events.Get(x => x.EventId == id);
            if (evnt1 == null)
            {
                return NotFound();
            }

            evnt1.Title = evnt.Title;
            evnt1.Description = evnt.Description;
            evnt1.ShortDesc = evnt.ShortDesc;
            evnt1.StartAt = evnt.StartAt;
            evnt1.EventLink = evnt.EventLink;
            evnt1.SubscriptionEvents = evnt.SubscriptionEvents;
            evnt1.EventCategories = evnt.EventCategories;

            try
            {
                await unitOfWork.Save();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UpdateEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }

            return NoContent();
        }

        // DELETE api/<EventController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(long id)
        {
            var evnt = await unitOfWork.Events.Get(x => x.EventId == id);

            if (evnt == null)
            {
                return NotFound();
            }

            await unitOfWork.Events.Delete(id);
            await unitOfWork.Save();

            return NoContent();
        }
    }
}
