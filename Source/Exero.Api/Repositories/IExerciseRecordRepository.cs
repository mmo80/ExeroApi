using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseRecordRepository
    {
        Task<ExerciseRecord> Get(Guid id);
        Task<ExerciseRecord> Add(ExerciseRecord exerciseRecord, Guid exerciseSessionId);
        Task<ExerciseRecord> Update(ExerciseRecord exerciseRecord);
        Task Remove(Guid id, Guid exerciseSessionId);
    }
}
