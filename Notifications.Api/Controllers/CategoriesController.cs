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
    public class CategoriesController : ControllerBase
    {
        NotificationsContext context;
        ILogger<CategoriesController> logger;

        public CategoriesController(NotificationsContext context, ILogger<CategoriesController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        // GET: api/<CategoriesController>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                logger.LogInformation($"Successfully executed {nameof(GetCategories)}");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetCategories)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();

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

            var ctgry = await context.Categories.FindAsync(id);
            if (ctgry == null)
            {
                return NotFound();
            }

            ctgry.CategoryName = category.CategoryName;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCategory)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }

            return NoContent();
        }

        // DELETE api/<CategoriesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
