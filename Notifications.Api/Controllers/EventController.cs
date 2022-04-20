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
using Telegram.Bot.Types;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> AddCategoryToEvent(long EventId, long CategoryId)
        {
            if (EventId < 1 || CategoryId < 1) return ApiResponse.BadRequest("Submitted data was invalid").ToResult();

            try
            {
                var @event = await notificationsService.AddCategoryToEvent(EventId, CategoryId);

                logger.LogInformation($"Successfully executed {nameof(AddCategoryToEvent)}");
                return @event.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(AddCategoryToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPut("{EventId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> AddCategoriesToEvent(long EventId, [FromBody] List<string> categories)
        {
            if (EventId < 1 || categories == null) return ApiResponse.BadRequest("Submitted data was invalid").ToResult();

            try
            {
                var @event = await notificationsService.AddCategoriesToEvent(EventId, categories);

                if (@event == null) return ApiResponse.NotFound().ToResult();

                logger.LogInformation($"Successfully executed {nameof(AddCategoryToEvent)}");
                return @event.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(AddCategoryToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPost("{EventId:long}/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SubscribeToEvent(long eventId, string telegramId)
        {
            if (!unitOfWork.Events.Exists(eventId)) return ApiResponse.NotFound("Event was not found").ToResult();
            if (string.IsNullOrEmpty(telegramId)) return ApiResponse.BadRequest("User ID is empty").ToResult();

            try
            {
                await notificationsService.SubscribeToEvent(eventId, telegramId);

                logger.LogInformation($"Successfully executed {nameof(SubscribeToEvent)}");
                return ApiResponse.Ok("Successfully subscribed").ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(SubscribeToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("{EventId:long}/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UnsubscribeToEvent(long EventId, string TelegramId)
        {
            if (!unitOfWork.Events.Exists(EventId) && string.IsNullOrEmpty(TelegramId)) return ApiResponse.BadRequest("Submitted data was invalid").ToResult();

            try
            {
                await notificationsService.UnsubscribeFromEvent(EventId, TelegramId);
                
                logger.LogInformation($"Successfully executed {nameof(UnsubscribeToEvent)}");
                return ApiResponse.Ok("Successfully unsubscribed from event").ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UnsubscribeToEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("Telegram/{TelegramId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListOfSubscribedEvents(string TelegramId)
        {
            if (string.IsNullOrEmpty(TelegramId)) return ApiResponse.BadRequest("Submitted data was invalid").ToResult();

            try
            {
                var events = await notificationsService.ListOfSubscribedEvents(TelegramId);

                logger.LogInformation($"Successfully executed {nameof(ListOfSubscribedEvents)}");
                return ApiResponse.Ok(events).ToResult();
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
        public async Task<ActionResult> Search([FromQuery] string search)
        {
            if (string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search)) return ApiResponse.BadRequest().ToResult();
            
            try
            {
                var events = await notificationsService.SearchEvents(search);

                logger.LogInformation($"Successfully executed {nameof(Search)}");
                return ApiResponse.Ok(events).ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(Search)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> FilterEvents([FromBody] List<string> filter)
        {
            if (filter == null) return ApiResponse.BadRequest().ToResult(); 

            try
            {
                var events = await notificationsService.FilterEvents(filter);
                if (events == null)
                    return ApiResponse.NotFound().ToResult();

                logger.LogInformation($"Successfully executed {nameof(FilterEvents)}");
                return events.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(FilterEvents)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("Order")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> OrderEvents()
        {
            try
            {
                var response = await notificationsService.GetOrderedByDateEvents();

                logger.LogInformation($"Successfully executed {nameof(OrderEvents)}");
                return response.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(OrderEvents)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}
