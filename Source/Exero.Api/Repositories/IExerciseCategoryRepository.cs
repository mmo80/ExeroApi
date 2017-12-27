using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseCategoryRepository
    {
        Task<IEnumerable<ExerciseCategory>> GetAll(Guid userId);
        Task<ExerciseCategory> Get(Guid userId, Guid id);
        Task<ExerciseCategory> Add(ExerciseCategory exerciseCategory);
        Task<ExerciseCategory> Update(Guid userId, Guid id, string name, string note);
        Task Remove(Guid userId, Guid id);
    }
}