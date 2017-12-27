using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAll(Guid userId, Guid categoryId);
        Task<Exercise> Get(Guid userId, Guid id);
        Task<Exercise> Add(Exercise exercise);
        Task<Exercise> Update(Guid userId, Guid categoryId, Guid id, string name, string note);
    }
}