using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseRepository
    {
        Task<List<Exercise>> ByGroup(Guid exerciseGroupId);
        Task<Exercise> Get(Guid id);
        Task<Exercise> Add(Exercise exercise, Guid exerciseGroupId);
        Task RelateExerciseToUser(Guid exerciseId, Guid userId);
        Task UpdateExerciseToUserSettings(Guid exerciseId, Guid userId, int restTimeInSeconds, string unit);
        Task<Exercise> Update(Exercise exercise);
    }
}