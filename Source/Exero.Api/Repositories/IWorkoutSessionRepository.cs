using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IWorkoutSessionRepository
    {
        Task<WorkoutSession> Get(Guid id);
        Task<List<WorkoutSession>> ByUser(Guid userId, DateTime from, DateTime to, int limit = 31);
        Task<WorkoutSession> Add(WorkoutSession workoutSession, Guid userId);
        Task<WorkoutSession> Update(WorkoutSession workoutSession);
        Task Remove(Guid id);
    }
}