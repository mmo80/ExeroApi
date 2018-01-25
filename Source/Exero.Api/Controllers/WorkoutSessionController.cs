using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Common;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class WorkoutSessionController : Controller
    {
        private readonly IWorkoutSessionRepository _workoutSessionRepository;

        public WorkoutSessionController(IWorkoutSessionRepository workoutSessionRepository)
        {
            _workoutSessionRepository = workoutSessionRepository;
        }

        [HttpGet("{userid:guid}/workoutsessions")]
        public async Task<IEnumerable<WorkoutSessionApi>> GetWorkoutSessions(
            Guid userId, [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int limit = 31)
        {
            var list = await _workoutSessionRepository.ByUser(userId, from, to, limit);
            return list.Select(x => new WorkoutSessionApi
            {
                Id = x.Id,
                Note = x.Note,
                StartDatetime = x.StartDatetime,
                EndDatetime = x.EndDatetime
            });
        }

        [HttpGet("{userid:guid}/workoutsessions/{id:guid}", Name = "GetWorkoutSession")]
        public async Task<WorkoutSessionApi> GetWorkoutSession(Guid userId, Guid id)
        {
            var workoutSession = await _workoutSessionRepository.Get(id);
            return new WorkoutSessionApi
            {
                Id = workoutSession.Id,
                Note = workoutSession.Note,
                StartDatetime = workoutSession.StartDatetime,
                EndDatetime = workoutSession.EndDatetime
            };
        }

        [HttpPost("{userid:guid}/workoutsessions")]
        public async Task<IActionResult> Post(
            Guid userId, [FromBody] WorkoutSessionUpdateApi workoutSessionUpdate)
        {
            var workoutSession = new WorkoutSession
            {
                Id = Guid.NewGuid(),
                Note = workoutSessionUpdate.Note,
                StartEpochTimestamp = workoutSessionUpdate.StartDatetime.ToEpoch()
            };

            var ws = await _workoutSessionRepository.Add(workoutSession, userId);

            return CreatedAtRoute("GetWorkoutSession",
                new { Controller = "WorkoutSession", userId, id = ws.Id },
                new WorkoutSessionApi
                {
                    Id = ws.Id,
                    Note = ws.Note,
                    StartDatetime = ws.StartDatetime,
                    EndDatetime = ws.EndDatetime
                });
        }

        [HttpPut("{userid:guid}/workoutsessions/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid userId, Guid id, [FromBody]WorkoutSessionUpdateApi workoutSessionUpdate)
        {
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
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}