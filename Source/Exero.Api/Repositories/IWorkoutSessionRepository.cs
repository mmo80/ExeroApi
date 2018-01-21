using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IWorkoutSessionRepository
    {
        Task<List<WorkoutSession>> ByUser(Guid userId, DateTime from, DateTime to, int limit = 31);
    }
}