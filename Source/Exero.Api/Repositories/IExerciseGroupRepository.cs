using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseGroupRepository
    {
        Task<List<ExerciseGroup>> ByCategory(Guid categoryId);
        Task<ExerciseGroup> Get(Guid id);
        Task<ExerciseGroup> Add(ExerciseGroup exerciseGroup, Guid categoryId);
        Task<ExerciseGroup> Update(ExerciseGroup exerciseGroup);
    }
}
