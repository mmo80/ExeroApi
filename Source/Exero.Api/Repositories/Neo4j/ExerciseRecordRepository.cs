using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exero.Api.Repositories.Neo4j
{
    public class ExerciseRecordRepository
    {
        private readonly IGraphRepository _graphRepository;

        public ExerciseRecordRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        //public async Task<>
    }
}
