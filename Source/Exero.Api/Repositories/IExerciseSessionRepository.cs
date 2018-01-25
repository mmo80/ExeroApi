using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseSessionRepository
    {
        Task<IList<ExerciseSession>> ByWorkoutSession(Guid workoutSessionId);
        Task<ExerciseSession> Get(Guid id);
        Task<ExerciseSession> Add(ExerciseSession exerciseSession, Guid exerciseId, Guid workoutSessionId);
    }
}