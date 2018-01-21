using System;
using System.Collections.Generic;
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

        public async Task<List<WorkoutSession>> ByUser(Guid userId, DateTime from, DateTime to, int limit = 31)
        {
            var list = new List<WorkoutSession>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (ws:WorkoutSession)-[r:BY_USER]->(u:User) 
                    WHERE u.id = $id and 
                        ws.startEpochTimestamp >= $startEpochTimestamp and
                        ws.endEpochTimestamp <= $endEpochTimestamp
                    RETURN ws.id, ws.note, ws.startEpochTimestamp, ws.endEpochTimestamp
                    LIMIT $limit",
                    new
                    {
                        id = userId.ToString(),
                        startEpochTimestamp = ToEpoch(from),
                        endEpochTimestamp = ToEpoch(to),
                        limit = limit
                    });

                while (await reader.FetchAsync())
                {
                    list.Add(new WorkoutSession()
                    {
                        Id = Guid.Parse(reader.Current[0].ToString()),
                        Note = reader.Current[1].ToString(),
                        StartEpochTimestamp = double.Parse(reader.Current[2].ToString()),
                        EndEpochTimestamp = double.Parse(reader.Current[3].ToString())
                    });
                }
            }
            return list;
        }

        // url: https://codereview.stackexchange.com/q/125275
        /// <summary>
        /// Converts a DateTime to the long representation which is the number of seconds since the unix epoch.
        /// </summary>
        /// <param name="dateTime">A DateTime to convert to epoch time.</param>
        /// <returns>The long number of seconds since the unix epoch.</returns>
        public static long ToEpoch(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
