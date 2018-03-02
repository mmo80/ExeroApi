using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Exero.Api.Common;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("")]
    public class ExerciseRecordController : Controller
    {
        private readonly IExerciseRecordRepository _exerciseRecordRepository;

        public ExerciseRecordController(IExerciseRecordRepository exerciseRecordRepository)
        {
            _exerciseRecordRepository = exerciseRecordRepository;
        }

        [HttpPost("exercisesessions/{exercisesessionid:guid}/records")]
        public async Task<IActionResult> Post(
            Guid exerciseSessionId, [FromBody] ExerciseRecordAddApi exerciseRecordAddApi)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var exerciseRecord = new ExerciseRecord
            {
                Id = Guid.NewGuid(),
                Set = exerciseRecordAddApi.Set,
                Reps = exerciseRecordAddApi.Reps,
                Value = exerciseRecordAddApi.Value,
                Unit = exerciseRecordAddApi.Unit,
                DropSet = exerciseRecordAddApi.DropSet,
                EpochTimestamp = exerciseRecordAddApi.Datetime.ToEpoch(),
                Note = exerciseRecordAddApi.Note
            };

            var er = await _exerciseRecordRepository.Add(exerciseRecord, exerciseSessionId);

            return CreatedAtRoute("GetExerciseSession",
                new { Controller = "ExerciseSession", id = exerciseSessionId },
                new ExerciseRecordApi
                {
                    Id = er.Id,
                    Set = er.Set,
                    Reps = er.Reps,
                    Value = er.Value,
                    Unit = er.Unit,
                    DropSet = er.DropSet,
                    Datetime = er.Datetime,
                    Note = er.Note
                });
        }

        [HttpPut("exercisesessions/{exercisesessionid:guid}/records/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid exerciseSessionId, Guid id, [FromBody] ExerciseRecordAddApi exerciseRecordAddApi)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var exerciseRecord = new ExerciseRecord
            {
                Id = Guid.NewGuid(),
                Set = exerciseRecordAddApi.Set,
                Reps = exerciseRecordAddApi.Reps,
                Value = exerciseRecordAddApi.Value,
                Unit = exerciseRecordAddApi.Unit,
                DropSet = exerciseRecordAddApi.DropSet,
                EpochTimestamp = exerciseRecordAddApi.Datetime.ToEpoch(),
                Note = exerciseRecordAddApi.Note
            };

            await _exerciseRecordRepository.Update(exerciseRecord);

            return NoContent();
        }

        [HttpDelete("exercisesessions/{exercisesessionid:guid}/records/{id:guid}")]
        public async Task<IActionResult> Delete(Guid exerciseSessionId, Guid id)
        {
            if (await _exerciseRecordRepository.Get(id) == null)
                return NotFound();

            await _exerciseRecordRepository.Remove(id, exerciseSessionId);
            return NoContent();
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
        public string Note { get; set; }
    }

    public class ExerciseRecordAddApi
    {
        [Required]
        public string Set { get; set; }

        [Required]
        public Int64 Reps { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public bool DropSet { get; set; }

        [Required]
        public DateTime Datetime { get; set; }

        public string Note { get; set; }
    }
}