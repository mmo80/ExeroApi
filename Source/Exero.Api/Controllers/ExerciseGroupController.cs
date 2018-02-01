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
    [Route("api")]
    public class ExerciseGroupController : Controller
    {
        private readonly IExerciseGroupRepository _exerciseGroupRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ExerciseGroupController(
            IExerciseGroupRepository exerciseGroupRepository, ICategoryRepository categoryRepository)
        {
            _exerciseGroupRepository = exerciseGroupRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("categories/{categoryId:guid}/exercisegroups")]
        public async Task<IActionResult> GetExerciseGroups(Guid categoryId)
        {
            var list = await _exerciseGroupRepository.ByCategory(categoryId);
            return Ok(list.Select(x => new ExerciseGroupApi { Id = x.Id, Name = x.Name, Note = x.Note }));
        }

        [HttpGet("exercisegroups/{id:guid}", Name = "GetExerciseGroup")]
        public async Task<IActionResult> GetExerciseGroup(Guid id)
        {
            var item = await _exerciseGroupRepository.Get(id);
            if (item == null)
                return NotFound();

            return Ok(new ExerciseGroupApi { Id = item.Id, Name = item.Name, Note = item.Note });
        }


        [HttpPost("categories/{categoryId:guid}/exercisegroups")]
        public async Task<IActionResult> Post(
            Guid categoryId, [FromBody] ExerciseGroupUpdateApi exerciseGroupUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if ((await _categoryRepository.Get(categoryId)) == null)
                return NotFound("Category not found.");

            var item = new ExerciseGroup
            {
                Id = Guid.NewGuid(),
                Name = exerciseGroupUpdate.Name,
                Note = exerciseGroupUpdate.Note
            };
            await _exerciseGroupRepository.Add(item, categoryId);

            return CreatedAtRoute("GetExerciseGroup",
                new { Controller = "ExerciseGroup", id = item.Id },
                new CategoryApi { Id = item.Id, Name = item.Name, Note = item.Note });
        }
    }

    public class ExerciseGroupApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
    }

    public class ExerciseGroupUpdateApi
    {
        public string Note { get; set; }

        [Required]
        public string Name { get; set; }
    }
}