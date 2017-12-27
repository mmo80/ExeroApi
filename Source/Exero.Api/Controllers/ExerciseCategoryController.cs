using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Route("api/user")]
    public class ExerciseCategoryController : Controller
    {
        private readonly IExerciseCategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public ExerciseCategoryController(IExerciseCategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        [HttpGet("{userid}/exercise/category")]
        public async Task<IEnumerable<CategoryApi>> GetCategories(Guid userId)
        {
            var categories = await _categoryRepository.GetAll(userId);
            return categories.Select(x => new CategoryApi { Id = x.Id, Name = x.Name, Note = x.Note });
        }

        [HttpGet("{userid}/exercise/category/{id}", Name = "GetCategory")]
        public async Task<CategoryApi> GetCategory(Guid userId, Guid id)
        {
            var category = await _categoryRepository.Get(userId, id);
            return new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note };
        }

        [HttpPost("{userid}/exercise/category")]
        public async Task<IActionResult> Post(Guid userId, [FromBody]CategoryUpdateApi categoryUpdate)
        {
            var user = await _userRepository.Get(userId);
            var category = new ExerciseCategory()
            {
                Id = Guid.NewGuid(),
                Name = categoryUpdate.Name,
                Note = categoryUpdate.Note,
                User = user
            };
            await _categoryRepository.Add(category);

            return CreatedAtRoute("GetCategory", 
                new { Controller = "ExerciseCategory", userId, id = category.Id }, 
                new CategoryApi { Id = category.Id, Name = category.Name, Note = category.Note });
        }

        [HttpPut("{userid}/exercise/category/{id}")]
        public async Task<IActionResult> Update(Guid userId, Guid id, [FromBody]CategoryUpdateApi categoryUpdate)
        {
            await _categoryRepository.Update(userId, id, categoryUpdate.Name, categoryUpdate.Note);
            return NoContent();
        }

        [HttpDelete("{userid}/exercise/category/{id}")]
        public async Task<IActionResult> Delete(Guid userId, Guid id)
        {
            await _categoryRepository.Remove(userId, id);
            return NoContent();
        }
    }


    public class CategoryUpdateApi
    {
        public string Note { get; set; }
        public string Name { get; set; }
    }


    public class CategoryApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
    }
}
