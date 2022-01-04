using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notifications.Api.IRepository;
using Notifications.DAL.Models;
using Notifications.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        readonly IUnitOfWork unitOfWork;
        readonly ILogger<CategoriesController> logger;
        readonly IMapper mapper;

        public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        // GET: api/<CategoriesController>
        [HttpGet]
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
        [HttpGet("{id:long}")]
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
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            await unitOfWork.Categories.Insert(category);
            await unitOfWork.Save();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        // PUT api/<CategoriesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(long id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            var ctgry = await unitOfWork.Categories.Get(x => x.CategoryId == id);
            if (ctgry == null)
            {
                return NotFound();
            }

            ctgry.CategoryName = category.CategoryName;

            try
            {
                await unitOfWork.Save();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCategory)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }

            return NoContent();
        }

        // DELETE api/<CategoriesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var category = await unitOfWork.Categories.Get(x => x.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            await unitOfWork.Categories.Delete(id);
            await unitOfWork.Save();

            return NoContent();
        }
    }
}
