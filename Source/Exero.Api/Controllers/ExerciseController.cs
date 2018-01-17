//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Exero.Api.Models;
//using Exero.Api.Repositories;
//using Microsoft.AspNetCore.Mvc;

//namespace Exero.Api.Controllers
//{
//    [Route("api/user")]
//    public class ExerciseController : Controller
//    {
//        //private readonly IExerciseRepository _exerciseRepository;
//        private readonly ICategoryRepository _categoryRepository;

//        public ExerciseController(IExerciseRepository exerciseRepository, ICategoryRepository categoryRepository)
//        {
//            _exerciseRepository = exerciseRepository;
//            _categoryRepository = categoryRepository;
//        }

//        [HttpGet("{userid}/category/{categoryid}/exercise")]
//        public async Task<IEnumerable<ExerciseApi>> GetExercises(Guid userId, Guid categoryId)
//        {
//            var exercises = await _exerciseRepository.GetAll(userId, categoryId);
//            return exercises.Select(x => new ExerciseApi { Id = x.Id, Name = x.Name, Note = x.Note });
//        }

//        [HttpGet("{userid}/category/{categoryid}/exercise/{id}", Name = "GetExercise")]
//        public async Task<ExerciseApi> GetExercise(Guid userId, Guid categoryid, Guid id)
//        {
//            var exercise = await _exerciseRepository.Get(userId, categoryid, id);
//            return new ExerciseApi { Id = exercise.Id, Name = exercise.Name, Note = exercise.Note };
//        }

//        [HttpPost("{userid}/category/{categoryid}/exercise")]
//        public async Task<IActionResult> Post(Guid userId, Guid categoryId, [FromBody]ExerciseUpdateApi exerciseUpdate)
//        {
//            var category = await _categoryRepository.Get(userId, categoryId);
//            var exercise = new Exercise()
//            {
//                Id = Guid.NewGuid(),
//                Name = exerciseUpdate.Name,
//                Note = exerciseUpdate.Note,
//                Category = category
//            };
//            await _exerciseRepository.Add(exercise);

//            return CreatedAtRoute("GetExercise",
//                new { Controller = "Exercise", userId, categoryId, id = exercise.Id },
//                new ExerciseApi { Id = exercise.Id, Name = exercise.Name, Note = exercise.Note });
//        }
//    }


//    public class ExerciseApi
//    {
//        public Guid Id { get; set; }
//        public string Note { get; set; }
//        public string Name { get; set; }
//    }


//    public class ExerciseUpdateApi
//    {
//        public string Note { get; set; }
//        public string Name { get; set; }
//    }
//}