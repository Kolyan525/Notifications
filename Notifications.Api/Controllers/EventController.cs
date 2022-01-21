using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [HttpGet("{id:long}", Name = "GetEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> GetEvent(long id)
        {
            try
            {
                var vent = await unitOfWork.Events.Get(x => x.EventId == id, new List<string> { "EventCategories" }); // TODO: fvent
                var result = mapper.Map<EventDTO>(vent);
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
                var vent = mapper.Map<Event>(eventDTO);
                await unitOfWork.Events.Insert(vent);
                await unitOfWork.Save();

                return CreatedAtRoute(nameof(GetEvent), new { id = vent.EventId }, vent);
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
                var vent = await unitOfWork.Events.Get(e => e.EventId == EventId);
                if (vent == null)
                {
                    logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateEvent)}");
                    return BadRequest("Submitted data is invalid");
                }

                mapper.Map(eventDTO, vent);
                unitOfWork.Events.Update(vent); // tracking question?
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
                var vent = await unitOfWork.Events.Get(x => x.EventId == id);
                if (vent == null)
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

        [HttpGet("/{id:long}", Name = "GetSpecialEvent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Event>> GetSpecialEvent(long id)
        {
            try
            {
                var vent = await unitOfWork.Events.Get(x => x.EventId == id, new List<string> { "SubscriptionEvents" }); // TODO: fvent
                var result = mapper.Map<EventDTO>(vent);
                logger.LogInformation($"Successfully executed {nameof(GetEvent)}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetEvent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}
