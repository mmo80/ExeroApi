using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories.Neo4j
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly IGraphRepository _graphRepository;

        public WorkoutSessionRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        // TODO: Add filter 'to' and 'from' epoch timestamp in WHERE clause
        public async Task<List<WorkoutSession>> ByUser(Guid userId, int limit = 31)
        {
            var list = new List<WorkoutSession>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (ws:WorkoutSession)-[r:BY_USER]->(u:User) 
                    WHERE u.id = $id
                    RETURN ws.id, ws.note, ws.startEpochTimestamp, ws.endEpochTimestamp
                    LIMIT $limit",
                    new { id = userId.ToString(), limit = limit });

                while (await reader.FetchAsync())
                {
                    list.Add(new WorkoutSession()
                    {
                        Id = Guid.Parse(reader.Current[0].ToString()),
                        Note = reader.Current[1].ToString(),
                        StartEpochTimestamp = (double)reader.Current[2],
                        EndEpochTimestamp = (double)reader.Current[3]
                    });
                }
            }
            return list;
        }
    }
}
