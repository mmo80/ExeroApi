using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class ExerciseGroupController : Controller
    {
        private readonly IExerciseGroupRepository _exerciseGroupRepository;

        public ExerciseGroupController(IExerciseGroupRepository exerciseGroupRepository)
        {
            _exerciseGroupRepository = exerciseGroupRepository;
        }

        [HttpGet("{userid:guid}/exercisegroups")]
        public async Task<IEnumerable<ExerciseGroupApi>> GetExerciseGroups(Guid userId, [FromBody] Guid categoryId)
        {
            var list = await _exerciseGroupRepository.ByCategory(categoryId);
            return list.Select(x => new ExerciseGroupApi { Id = x.Id, Name = x.Name, Note = x.Note });
        }

        [HttpGet("{userid:guid}/exercisegroups/{id:guid}", Name = "GetExerciseGroup")]
        public async Task<ExerciseGroupApi> GetExerciseGroup(Guid userId, Guid id)
        {
            var item = await _exerciseGroupRepository.Get(id);
            return new ExerciseGroupApi { Id = item.Id, Name = item.Name, Note = item.Note };
        }
    }

    public class ExerciseGroupApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
    }
}