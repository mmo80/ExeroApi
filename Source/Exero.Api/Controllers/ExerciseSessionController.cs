using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValueType = Exero.Api.Common.ValueType;

namespace Exero.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api")]
    public class ExerciseSessionController : Controller
    {
        private readonly IExerciseSessionRepository _exerciseSessionRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IUserRepository _userRepository;

        public ExerciseSessionController(IExerciseSessionRepository exerciseSessionRepository,
            IExerciseRepository exerciseRepository,
            IUserRepository userRepository)
        {
            _exerciseSessionRepository = exerciseSessionRepository;
            _exerciseRepository = exerciseRepository;
            _userRepository = userRepository;
        }

        [HttpGet("exercisesessions")]
        public async Task<IActionResult> GetExerciseSessions(
            [FromQuery] Guid workoutSessionId)
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
                    Datetime = r.Datetime,
                    Note = r.Note
                })
            }));
        }

        [HttpGet("exercisesessions/{id:guid}", Name = "GetExerciseSession")]
        public async Task<IActionResult> GetExerciseSession(Guid id)
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
                    Datetime = r.Datetime,
                    Note = r.Note
                })
            });
        }

        [HttpPost("exercisesessions")]
        public async Task<IActionResult> Post(
            [FromBody] ExerciseSessionAddApi exerciseSessionAdd)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var exerciseSession = new ExerciseSession
            {
                Id = Guid.NewGuid(),
                Note = exerciseSessionAdd.Note
            };

            var user = await _userRepository.ByEmail(Common.ClaimsHelper.GetUserValue(User, ValueType.email));
            if (user.Id == Guid.Empty)
                return BadRequest("No User Found.");

            var es = await _exerciseSessionRepository.Add(exerciseSession, exerciseSessionAdd.ExerciseId,
                exerciseSessionAdd.WorkoutSessionId);

            await _exerciseRepository.RelateExerciseToUser(exerciseSessionAdd.ExerciseId, user.Id);

            return CreatedAtRoute("GetExerciseSession",
                new { Controller = "ExerciseSession", id = es.Id },
                new ExerciseSessionApi
                {
                    Id = es.Id,
                    Note = es.Note,
                    ExerciseName = es.ExerciseName,
                    Records = new List<ExerciseSessionApi.ExerciseRecord>()
                });
        }

        [HttpPut("exercisesessions/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody]ExerciseSessionUpdateApi exerciseSessionUpdate)
        {
            if (await _exerciseSessionRepository.Get(id) == null)
                return NotFound();

            var exerciseSession = new ExerciseSession
            {
                Id = id,
                Note = exerciseSessionUpdate.Note
            };

            await _exerciseSessionRepository.Update(exerciseSession);
            return NoContent();
        }

        [HttpDelete("exercisesessions/{id:guid}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            if (await _exerciseSessionRepository.Get(id) == null)
                return NotFound();

            await _exerciseSessionRepository.Remove(id);
            return NoContent();
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
            public string Note { get; set; }
        }
    }

    public class ExerciseSessionAddApi
    {
        [Required]
        public Guid ExerciseId { get; set; }

        [Required]
        public Guid WorkoutSessionId { get; set; }

        public string Note { get; set; }
    }

    public class ExerciseSessionUpdateApi
    {
        public string Note { get; set; }
    }
}