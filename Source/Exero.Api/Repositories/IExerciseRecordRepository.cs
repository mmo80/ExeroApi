using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IExerciseRecordRepository
    {
        Task<ExerciseRecord> Add(ExerciseRecord exerciseRecord, Guid exerciseSessionId);
    }
}
