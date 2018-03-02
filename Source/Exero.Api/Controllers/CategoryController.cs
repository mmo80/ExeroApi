using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetAll();
            return Ok(categories.Select(x => new CategoryApi { Id = x.Id, Name = x.Name, Note = x.Note }));
        }

        [HttpGet("categories/{id:guid}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await _categoryRepository.Get(id);
            if (category == null)
                return NotFound();
            
            return Ok(new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note });
        }

        [HttpPost("categories")]
        public async Task<IActionResult> Post([FromBody]CategoryUpdateApi categoryUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryUpdate.Name,
                Note = categoryUpdate.Note
            };
            await _categoryRepository.Add(category);

            return CreatedAtRoute("GetCategory", 
                new { Controller = "Category", id = category.Id }, 
                new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note });
        }

        [HttpPut("categories/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody]CategoryUpdateApi categoryUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!await CategoryExists(id))
                return NotFound();

            await _categoryRepository.Update(new Category()
                { Id = id, Name = categoryUpdate.Name, Note = categoryUpdate.Note });
            return NoContent();
        }

        [HttpDelete("categories/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await CategoryExists(id))
                return NotFound();

            await _categoryRepository.Remove(id);
            return NoContent();
        }


        private async Task<bool> CategoryExists(Guid id)
        {
            var category = await _categoryRepository.Get(id);
            return (category != null);
        }
    }


    public class CategoryUpdateApi
    {
        public string Note { get; set; }

        [Required]
        public string Name { get; set; }
    }


    public class CategoryApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
    }
}
