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
    public class WorkoutSessionController : Controller
    {
        private readonly IWorkoutSessionRepository _workoutSessionRepository;

        public WorkoutSessionController(IWorkoutSessionRepository workoutSessionRepository)
        {
            _workoutSessionRepository = workoutSessionRepository;
        }

        [HttpGet("{userid:guid}/workoutsessions")]
        public async Task<IEnumerable<WorkoutSessionApi>> GetWorkoutSessions(
            Guid userId, 
            [FromQuery] DateTime from, 
            [FromQuery] DateTime to, 
            [FromQuery] int limit = 31)
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
    }

    public class WorkoutSessionApi
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
    }
}