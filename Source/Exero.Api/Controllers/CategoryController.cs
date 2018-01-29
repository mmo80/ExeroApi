using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("{userid:guid}/categories")]
        public async Task<IActionResult> GetCategories(Guid userId)
        {
            var categories = await _categoryRepository.GetAll(userId);
            return Ok(categories.Select(x => new CategoryApi { Id = x.Id, Name = x.Name, Note = x.Note }));
        }

        [HttpGet("{userid:guid}/categories/{id:guid}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(Guid userId, Guid id)
        {
            var category = await _categoryRepository.Get(userId, id);
            if (category == null)
                return NotFound();
            
            return Ok(new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note });
        }

        [HttpPost("{userid:guid}/categories")]
        public async Task<IActionResult> Post(Guid userId, [FromBody]CategoryUpdateApi categoryUpdate)
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
                new { Controller = "Category", userId, id = category.Id }, 
                new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note });
        }

        [HttpPut("{userid:guid}/categories/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid userId, Guid id, [FromBody]CategoryUpdateApi categoryUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!await CategoryExists(userId, id))
                return NotFound();

            await _categoryRepository.Update(userId, new Category()
                { Id = id, Name = categoryUpdate.Name, Note = categoryUpdate.Note });
            return NoContent();
        }

        [HttpDelete("{userid:guid}/categories/{id:guid}")]
        public async Task<IActionResult> Delete(Guid userId, Guid id)
        {
            if (!await CategoryExists(userId, id))
                return NotFound();

            await _categoryRepository.Remove(userId, id);
            return NoContent();
        }


        private async Task<bool> CategoryExists(Guid userId, Guid id)
        {
            var category = await _categoryRepository.Get(userId, id);
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
