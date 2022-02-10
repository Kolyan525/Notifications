using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notifications.BL.IRepository;
using Notifications.BL.Services;
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
        readonly NotificationsService notificationsService;
        public EventController(IUnitOfWork unitOfWork, ILogger<EventController> logger, IMapper mapper, NotificationsService notificationsService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
            this.notificationsService = notificationsService;
        }

        // GET: api/<EventController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents([FromQuery] RequestParams requestParams)
        {
            try
            {
                // TODO: check size 1, page 10 content in events variable
                var events = await unitOfWork.Events.GetAll(requestParams);
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
        [HttpGet("{id:long}", Name = "GetEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> GetEvent(long id)
        {
            try
            {
                var @event = await unitOfWork.Events.Get(x => x.EventId == id, new List<string> { "EventCategories", "SubscriptionEvents" });
                var result = mapper.Map<EventDTO>(@event);
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDTO eventDTO) // TODO: DTO
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid POST attempt in {nameof(CreateEvent)}");
                return BadRequest(ModelState);
            }

            try
            {
                var @event = mapper.Map<Event>(eventDTO);
                await unitOfWork.Events.Insert(@event);
                await unitOfWork.Save();

                return CreatedAtRoute(nameof(GetEvent), new { id = @event.EventId }, @event);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(CreateEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // PUT api/<EventController>/5
        [HttpPut("{EventId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEvent(long EventId, UpdateEventDTO eventDTO)
        {
            if (!ModelState.IsValid || EventId < 1)
            {
                logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateEvent)}");
                return BadRequest(ModelState);
            }

            try
            {
                var @event = await unitOfWork.Events.Get(e => e.EventId == EventId);
                if (@event == null)
                {
                    logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateEvent)}");
                    return BadRequest("Submitted data is invalid");
                }

                mapper.Map(eventDTO, @event);
                unitOfWork.Events.Update(@event); // tracking question?
                await unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UpdateEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // DELETE api/<EventController>/5
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEvent(long id)
        {
            if (id < 1)
            {
                logger.LogError($"Invalid DELETE attempt in {nameof(DeleteEvent)}");
                return BadRequest("Submitted data is invalid");
            }

            try
            {
                var @event = await unitOfWork.Events.Get(x => x.EventId == id);
                if (@event == null)
                {
                    logger.LogError($"Invalid DELETE attempt in {nameof(DeleteEvent)}");
                    return BadRequest("Submitted data is invalid");
                }

                await unitOfWork.Events.Delete(id);
                await unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Invalid DELETE attempt in {nameof(DeleteEvent)}");
                return BadRequest("Submitted data is invalid");
            }
        }

        [HttpPut("{EventId:long}/{CategoryId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> AddCategoryToEvent(long EventId, long CategoryId)
        {
            try
            {
                if (!unitOfWork.Events.Exists(EventId)) return StatusCode(409, "Event doesn't exists.");
                if (!unitOfWork.Categories.Exists(CategoryId)) return StatusCode(409, "Category doesn't exists.");

                var @event = await notificationsService.AddCategoryToEvent(EventId, CategoryId);
                if (@event == null)
                {
                    return StatusCode(409, "Already exists.");
                }
                unitOfWork.Events.Update(@event); // tracking question?
                await unitOfWork.Save();
                var result = mapper.Map<EventDTO>(@event);
                logger.LogInformation($"Successfully executed {nameof(AddCategoryToEvent)}");
                return Ok(@event);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(AddCategoryToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPost("{EventId:long}/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Subscription>> SubscribeToEvent(long EventId, string TelegramId)
        {
            try
            {
                var sub = await notificationsService.Subscribe(EventId, TelegramId);
                if (sub == null)
                {
                    return StatusCode(409, "Something went wrong");
                }
                logger.LogInformation($"Successfully executed {nameof(SubscribeToEvent)}");
                return Ok(sub);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(SubscribeToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("{EventId:long}/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Subscription>> UnsubscribeToEvent(long EventId, string TelegramId)
        {
            try
            {
                var sub = await notificationsService.Unsubscribe(EventId, TelegramId);
                if (sub == null)
                {
                    return StatusCode(409, "Something went wrong");
                }
                logger.LogInformation($"Successfully executed {nameof(UnsubscribeToEvent)}");
                return sub;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UnsubscribeToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("Telegram/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Subscription>> ListOfSubscribedEvents(string TelegramId)
        {
            try
            {
                var events = await notificationsService.ListOfSubscribedEvents(TelegramId);
                if (events == null)
                {
                    return StatusCode(409, "Something went wrong");
                }
                logger.LogInformation($"Successfully executed {nameof(ListOfSubscribedEvents)}");
                return Ok(events);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(ListOfSubscribedEvents)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        //[HttpGet("Search/{Search}")]
        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Seacrh([FromQuery] string Search)
        {
            try
            {
                var events = await notificationsService.SearchEvents(Search);
                if (events == null)
                    return NoContent();

                logger.LogInformation($"Successfully executed {nameof(SubscribeToEvent)}");
                return Ok(events);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(SubscribeToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}
