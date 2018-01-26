using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class ExerciseSessionController : Controller
    {
        private readonly IExerciseSessionRepository _exerciseSessionRepository;
        private readonly IExerciseRepository _exerciseRepository;

        public ExerciseSessionController(IExerciseSessionRepository exerciseSessionRepository,
            IExerciseRepository exerciseRepository)
        {
            _exerciseSessionRepository = exerciseSessionRepository;
            _exerciseRepository = exerciseRepository;
        }

        [HttpGet("{userid:guid}/exercisesessions")]
        public async Task<IActionResult> GetExerciseSessions(
            Guid userId, [FromQuery] Guid workoutSessionId)
        {
            var list = await _exerciseSessionRepository.ByWorkoutSession(workoutSessionId);
            return Ok(list.Select(x => new ExerciseSessionApi
            {
                Id = x.Id,
                Note = x.Note,
                ExerciseName = x.ExerciseName,
                Records = x.Records.Select(r => new ExerciseSessionApi.ExerciseRecord
                {
                    Set = r.Set,
                    Reps = r.Reps,
                    Value = r.Value,
                    Unit = r.Unit,
                    DropSet = r.DropSet,
                    Datetime = r.Datetime
                })
            }));
        }

        [HttpGet("{userid:guid}/exercisesessions/{id:guid}", Name = "GetExerciseSession")]
        public async Task<IActionResult> GetExerciseSession(Guid userId, Guid id)
        {
            var item = await _exerciseSessionRepository.Get(id);
            return Ok(new ExerciseSessionApi
            {
                Id = item.Id,
                Note = item.Note,
                ExerciseName = item.ExerciseName,
                Records = item.Records.Select(r => new ExerciseSessionApi.ExerciseRecord
                {
                    Set = r.Set,
                    Reps = r.Reps,
                    Value = r.Value,
                    Unit = r.Unit,
                    DropSet = r.DropSet,
                    Datetime = r.Datetime
                })
            });
        }

        [HttpPost("{userid:guid}/exercisesessions")]
        public async Task<IActionResult> Post(
            Guid userId, [FromBody] ExerciseSessionUpdateApi exerciseSessionUpdate)
        {
            var exerciseSession = new ExerciseSession
            {
                Id = Guid.NewGuid(),
                Note = exerciseSessionUpdate.Note
            };

            var es = await _exerciseSessionRepository.Add(exerciseSession, exerciseSessionUpdate.ExerciseId,
                exerciseSessionUpdate.WorkoutSessionId);

            await _exerciseRepository.RelateExerciseToUser(exerciseSessionUpdate.ExerciseId, userId);

            return CreatedAtRoute("GetExerciseSession",
                new { Controller = "ExerciseSession", userId, id = es.Id },
                new ExerciseSessionApi
                {
                    Id = es.Id,
                    Note = es.Note,
                    ExerciseName = es.ExerciseName,
                    Records = new List<ExerciseSessionApi.ExerciseRecord>()
                });
        }
    }


    public class ExerciseSessionApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public string ExerciseName { get; set; }
        public IEnumerable<ExerciseRecord> Records { get; set; }

        public class ExerciseRecord
        {
            public string Set { get; set; }
            public Int64 Reps { get; set; }
            public double Value { get; set; }
            public string Unit { get; set; }
            public bool DropSet { get; set; }
            public DateTime Datetime { get; set; }
        }
    }

    public class ExerciseSessionUpdateApi
    {
        public Guid ExerciseId { get; set; }
        public Guid WorkoutSessionId { get; set; }
        public string Note { get; set; }
        public string ExerciseName { get; set; }
    }
}