using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Common;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly IGraphRepository _graphRepository;

        public WorkoutSessionRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<WorkoutSession> Get(Guid id)
        {
            WorkoutSession item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (ws:WorkoutSession) WHERE ws.id = $id 
                    RETURN ws.id, ws.note, ws.startEpochTimestamp, ws.endEpochTimestamp",
                    new { id = id.ToString() }
                );
                item = await GetWorkoutSession(reader);
            }
            return item;
        }

        public async Task<List<WorkoutSession>> ByUser(
            Guid userId, DateTime from, DateTime to, int limit = 31)
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
                        startEpochTimestamp = from.ToEpoch(),
                        endEpochTimestamp = to.ToEpoch(),
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

        public async Task<WorkoutSession> Add(WorkoutSession workoutSession, Guid userId)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (u:User { id = $userId })
                    CREATE (ws:WorkoutSession { id: $id, note: $note, startEpochTimestamp: $startEpochTimestamp, endEpochTimestamp: 0 }),
                    (ws)-[:BY_USER]->(u)
                    RETURN ws.id, ws.note, ws.startEpochTimestamp, ws.endEpochTimestamp",
                    new
                    {
                        userId = userId,
                        id = workoutSession.Id.ToString(),
                        note = workoutSession.Note,
                        startEpochTimestamp = workoutSession.StartEpochTimestamp
                    }
                );
                workoutSession = await GetWorkoutSession(reader);
            }
            return workoutSession;
        }

        public async Task<WorkoutSession> Update(WorkoutSession workoutSession)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (ws:WorkoutSession { id = $id }) 
                    SET ws.name = $name, ws.note = $note, 
                    ws.startEpochTimestamp = $startEpochTimestamp, ws.endEpochTimestamp = $endEpochTimestamp
                    RETURN ws.id, ws.note, ws.startEpochTimestamp, ws.endEpochTimestamp",
                    new
                    {
                        id = workoutSession.Id.ToString(),
                        note = workoutSession.Note,
                        startEpochTimestamp = workoutSession.StartEpochTimestamp,
                        endEpochTimestamp = workoutSession.EndEpochTimestamp
                    }
                );
                workoutSession = await GetWorkoutSession(reader);
            }
            return workoutSession;
        }


        private async Task<WorkoutSession> GetWorkoutSession(IStatementResultCursor reader)
        {
            WorkoutSession item = null;
            while (await reader.FetchAsync())
            {
                item = new WorkoutSession()
                {
                    Id = Guid.Parse(reader.Current[0].ToString()),
                    Note = reader.Current[1]?.ToString(),
                    StartEpochTimestamp = double.Parse(reader.Current[2].ToString()),
                    EndEpochTimestamp = (reader.Current[3] != null) ? 
                        double.Parse(reader.Current[3].ToString()) : 0
                };
            }
            return item;
        }
    }
}
