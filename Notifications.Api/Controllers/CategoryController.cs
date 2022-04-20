using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CategoryController : ControllerBase
    {
        readonly IUnitOfWork unitOfWork;
        readonly ILogger<CategoryController> logger;
        readonly IMapper mapper;
        readonly NotificationsService notificationsService;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger, IMapper mapper, NotificationsService notificationsService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
            this.notificationsService = notificationsService;
        }

        // GET: api/<CategoriesController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await unitOfWork.Categories.GetAll();
                var results = mapper.Map<IList<CategoryDTO>>(categories);
                logger.LogInformation($"Successfully executed {nameof(GetCategories)}");
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetCategories)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id:long}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            try
            {
                var category = await unitOfWork.Categories.Get(x => x.CategoryId == id, new List<string> { "EventCategories" });
                var result = mapper.Map<CategoryDTO>(category);
                logger.LogInformation($"Successfully executed {nameof(GetCategories)}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetCategory)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // POST api/<CategoriesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDTO) // TODO: DTO
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid POST attempt in {nameof(CreateCategory)}");
                return BadRequest(ModelState);
            }

            try
            {
                var category = mapper.Map<Category>(categoryDTO);
                await unitOfWork.Categories.Insert(category);
                await unitOfWork.Save();

                return CreatedAtRoute(nameof(GetCategory), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(CreateCategory)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // PUT api/<CategoriesController>/5
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory(long id, UpdateCategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCategory)}");
                return BadRequest(ModelState);
            }

            try
            {
                var category = await unitOfWork.Categories.Get(c => c.CategoryId == id);
                if (category == null)
                {
                    logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCategory)}");
                    return BadRequest("Submitted data is invalid");
                }

                mapper.Map(categoryDTO, category);
                unitOfWork.Categories.Update(category);
                await unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCategory)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        // DELETE api/<CategoriesController>/5
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            if (id < 1)
            {
                logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCategory)}");
                return BadRequest("Submitted data is invalid");
            }

            try
            {
                var category = await unitOfWork.Categories.Get(x => x.CategoryId == id);
                if (category == null)
                {
                    logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCategory)}");
                    return BadRequest("Submitted data is invalid");
                }

                await unitOfWork.Categories.Delete(id);
                await unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Invalid DELETE attempt in {nameof(DeleteCategory)}");
                return BadRequest("Submitted data is invalid");
            }
        }

        [HttpGet("GetCategoryEvents/{id:long}", Name = "GetCategoryEvents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Category>> GetCategoryEvents(long id)
        {
            try
            {
                var category = await unitOfWork.Categories.GetFirstOrDefault(
                    x => x.CategoryId == id, 
                    include: x => x
                        .Include(x => x.EventCategories)
                        .ThenInclude(x => x.Event)
                );

                if (category == null)
                    return BadRequest();
                
                var events = new List<Event>();
                foreach (var @event in category.EventCategories)
                    events.Add(@event.Event);

                var results = mapper.Map<IList<CreateEventDTO>>(events);
                //var result = mapper.Map<EventDTO>(events);
                logger.LogInformation($"Successfully executed {nameof(GetCategoryEvents)}");
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetCategoryEvents)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPost("Subscribe/{id:long}/{userId}", Name = "Subscribe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Subscribe(long id, string userId)
        {
            try
            {
                var result = await notificationsService.SubscribeToCategory(id, userId);
                
                logger.LogInformation($"Successfully executed {nameof(Subscribe)}");
                return result.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(Subscribe)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
        
        [HttpGet("Unsubscribe/{id:long}/{userId}", Name = "Unsubscribe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Unsubscribe(long id, string userId)
        {
            try
            {
                var result = await notificationsService.UnsubscribeFromCategory(id, userId);
                
                logger.LogInformation($"Successfully executed {nameof(Unsubscribe)}");
                return result.ToResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(Unsubscribe)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}
