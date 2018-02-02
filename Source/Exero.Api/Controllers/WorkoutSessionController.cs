using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    [Route("api")]
    public class WorkoutSessionController : BaseController
    {
        private readonly IWorkoutSessionRepository _workoutSessionRepository;

        public WorkoutSessionController(IWorkoutSessionRepository workoutSessionRepository,
            IUserRepository userRepository) : base(userRepository)
        {
            _workoutSessionRepository = workoutSessionRepository;
        }

        [HttpGet("workoutsessions")]
        public async Task<IActionResult> GetWorkoutSessions(
            [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int limit = 31)
        {
            var userResult = await CheckUser();
            if (userResult.NotExist)
                return BadRequest("No User Found.");

            var list = await _workoutSessionRepository.ByUser(userResult.User.Id, from, to, limit);
            return Ok(list.Select(x => new WorkoutSessionApi
            {
                Id = x.Id,
                Note = x.Note,
                StartDatetime = x.StartDatetime,
                EndDatetime = x.EndDatetime
            }));
        }

        [HttpGet("workoutsessions/{id:guid}", Name = "GetWorkoutSession")]
        public async Task<IActionResult> GetWorkoutSession(Guid id)
        {
            var workoutSession = await _workoutSessionRepository.Get(id);
            return Ok(new WorkoutSessionApi
            {
                Id = workoutSession.Id,
                Note = workoutSession.Note,
                StartDatetime = workoutSession.StartDatetime,
                EndDatetime = workoutSession.EndDatetime
            });
        }

        [HttpPost("workoutsessions")]
        public async Task<IActionResult> Post(
            [FromBody] WorkoutSessionUpdateApi workoutSessionUpdate)
        {
            var workoutSession = new WorkoutSession
            {
                Id = Guid.NewGuid(),
                Note = workoutSessionUpdate.Note,
                StartEpochTimestamp = workoutSessionUpdate.StartDatetime.ToEpoch()
            };

            var userResult = await CheckUser();
            if (userResult.NotExist)
                return BadRequest("No User Found.");

            var ws = await _workoutSessionRepository.Add(workoutSession, userResult.User.Id);

            return CreatedAtRoute("GetWorkoutSession",
                new { Controller = "WorkoutSession", id = ws.Id },
                new WorkoutSessionApi
                {
                    Id = ws.Id,
                    Note = ws.Note,
                    StartDatetime = ws.StartDatetime,
                    EndDatetime = ws.EndDatetime
                });
        }

        [HttpPut("workoutsessions/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id, [FromBody]WorkoutSessionUpdateApi workoutSessionUpdate)
        {
            if (await _workoutSessionRepository.Get(id) == null)
                return NotFound();

            var workoutSession = new WorkoutSession
            {
                Id = id,
                Note = workoutSessionUpdate.Note,
                StartEpochTimestamp = workoutSessionUpdate.StartDatetime.ToEpoch(),
                EndEpochTimestamp = workoutSessionUpdate.EndDatetime.ToEpoch()
            };

            await _workoutSessionRepository.Update(workoutSession);
            return NoContent();
        }

        [HttpDelete("workoutsessions/{id:guid}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            if (await _workoutSessionRepository.Get(id) == null)
                return NotFound();

            await _workoutSessionRepository.Remove(id);
            return NoContent();
        }
    }


    public class WorkoutSessionApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }

    public class WorkoutSessionUpdateApi
    {
        public string Note { get; set; }

        [Required]
        public DateTime StartDatetime { get; set; }

        public DateTime EndDatetime { get; set; }
    }
}