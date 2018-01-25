using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Common;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class ExerciseRecordController : Controller
    {
        private readonly IExerciseRecordRepository _exerciseRecordRepository;

        public ExerciseRecordController(IExerciseRecordRepository exerciseRecordRepository)
        {
            _exerciseRecordRepository = exerciseRecordRepository;
        }

        [HttpPost("{userid:guid}/exercisesessions/{exercisesessionid:guid}/record")]
        public async Task<IActionResult> Post(
            Guid userId, Guid exerciseSessionId, [FromBody] ExerciseRecordUpdateApi exerciseRecordUpdateApi)
        {
            var exerciseRecord = new ExerciseRecord
            {
                Id = Guid.NewGuid(),
                Set = exerciseRecordUpdateApi.Set,
                Reps = exerciseRecordUpdateApi.Reps,
                Value = exerciseRecordUpdateApi.Value,
                Unit = exerciseRecordUpdateApi.Unit,
                DropSet = exerciseRecordUpdateApi.DropSet,
                EpochTimestamp = exerciseRecordUpdateApi.Datetime.ToEpoch()
            };

            var er = await _exerciseRecordRepository.Add(exerciseRecord, exerciseSessionId);

            return CreatedAtRoute("GetExerciseSession",
                new { Controller = "ExerciseSession", userId, id = exerciseSessionId },
                new ExerciseRecordApi
                {
                    Id = er.Id,
                    Set = er.Set,
                    Reps = er.Reps,
                    Value = er.Value,
                    Unit = er.Unit,
                    DropSet = er.DropSet,
                    Datetime = er.Datetime
                });
        }
    }

    public class ExerciseRecordApi
    {
        public Guid Id { get; set; }
        public string Set { get; set; }
        public Int64 Reps { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public bool DropSet { get; set; }
        public DateTime Datetime { get; set; }
    }

    public class ExerciseRecordUpdateApi
    {
        public string Set { get; set; }
        public Int64 Reps { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public bool DropSet { get; set; }
        public DateTime Datetime { get; set; }
    }
}