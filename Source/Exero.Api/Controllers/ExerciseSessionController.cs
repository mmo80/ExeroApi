using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class ExerciseSessionController : Controller
    {
        private readonly IExerciseSessionRepository _exerciseSessionRepository;

        public ExerciseSessionController(IExerciseSessionRepository exerciseSessionRepository)
        {
            _exerciseSessionRepository = exerciseSessionRepository;
        }

        [HttpGet("{userid:guid}/exercisesessions")]
        public async Task<IEnumerable<ExerciseSessionApi>> GetExerciseSessions(Guid userId, [FromQuery] Guid workoutSessionId)
        {
            var list = await _exerciseSessionRepository.ByWorkoutSession(workoutSessionId);
            return list.Select(x => new ExerciseSessionApi
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
}