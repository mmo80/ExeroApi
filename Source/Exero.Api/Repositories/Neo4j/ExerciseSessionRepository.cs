using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class ExerciseSessionRepository : IExerciseSessionRepository
    {
        private readonly IGraphRepository _graphRepository;

        public ExerciseSessionRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<IList<ExerciseSession>> ByWorkoutSession(Guid workoutSessionId)
        {
            var list = new List<ExerciseSession>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (er:ExerciseRecord)-[r:FOR_EXERCISE_SESSION]->(es:ExerciseSession)-[:FOR_EXERCISE]->(e:Exercise)
                    MATCH (es)-[:FOR_WORKOUT_SESSION]->(ws:WorkoutSession)
                    WHERE ws.id = $id
                    RETURN es.id, es.note, e.name, er.id, er.epochTimestamp, er.set, er.reps, er.value, er.unit, er.dropSet
                    ORDER BY er.epochTimestamp",
                    new { id = workoutSessionId.ToString() });


                var exerciseSession = new ExerciseSession();
                while (await reader.FetchAsync())
                {
                    var exerciseSessionId = Guid.Parse(reader.Current[0].ToString());
                    if (exerciseSession.Id != exerciseSessionId)
                    {
                        exerciseSession = new ExerciseSession()
                        {
                            Id = exerciseSessionId,
                            Note = reader.Current[1].ToString(),
                            ExerciseName = reader.Current[2].ToString(),
                            Records = new List<ExerciseRecord>()
                        };
                        list.Add(exerciseSession);
                    }
                    exerciseSession.Records.Add(new ExerciseRecord()
                    {
                        Id = Guid.Parse(reader.Current[3].ToString()),
                        EpochTimestamp = double.Parse(reader.Current[4].ToString()),
                        Set = reader.Current[5].ToString(),
                        Reps = (Int64)reader.Current[6],
                        Value = double.Parse(reader.Current[7].ToString()),
                        Unit = reader.Current[8].ToString(),
                        DropSet = (bool)reader.Current[9]
                    });
                }
            }
            return list;
        }

        public async Task<ExerciseSession> Get(Guid id)
        {
            ExerciseSession item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (er:ExerciseRecord)-[r:FOR_EXERCISE_SESSION]->(es:ExerciseSession)-[:FOR_EXERCISE]->(e:Exercise)
                    WHERE es.id = $id
                    RETURN es.id, es.note, e.name, er.id, er.epochTimestamp, er.set, er.reps, er.value, er.unit, er.dropSet
                    ORDER BY er.epochTimestamp",
                    new { id = id.ToString() }
                );
                item = await GetExerciseSessionComplete(reader);
            }
            return item;
        }

        public async Task<ExerciseSession> Add(
            ExerciseSession exerciseSession, Guid exerciseId, Guid workoutSessionId)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (e:Exercise { id = $exerciseId })
                    MATCH (ws:WorkoutSession { id = $workoutSessionId })
                    CREATE (es:ExerciseSession { id: $id, note: $note }),
                    (es)-[:FOR_EXERCISE]->(e),
                    (es)-[:FOR_WORKOUT_SESSION]->(ws)
                    RETURN es.id, es.note, e.name",
                    new
                    {
                        exerciseId = exerciseId,
                        workoutSessionId = workoutSessionId,
                        id = exerciseSession.Id.ToString(),
                        note = exerciseSession.Note
                    }
                );
                exerciseSession = await GetExerciseSession(reader);
            }
            return exerciseSession;
        }


        private async Task<ExerciseSession> GetExerciseSession(IStatementResultCursor reader)
        {
            ExerciseSession item = null;
            while (await reader.FetchAsync())
            {
                item = new ExerciseSession()
                {
                    Id = Guid.Parse(reader.Current[0].ToString()),
                    Note = reader.Current[1].ToString(),
                    ExerciseName = reader.Current[2].ToString()
                };
            }
            return item;
        }

        private async Task<ExerciseSession> GetExerciseSessionComplete(IStatementResultCursor reader)
        {
            var exerciseSession = new ExerciseSession();
            while (await reader.FetchAsync())
            {
                var exerciseSessionId = Guid.Parse(reader.Current[0].ToString());
                if (exerciseSession.Id != exerciseSessionId)
                {
                    exerciseSession = new ExerciseSession()
                    {
                        Id = exerciseSessionId,
                        Note = reader.Current[1].ToString(),
                        ExerciseName = reader.Current[2].ToString(),
                        Records = new List<ExerciseRecord>()
                    };
                }
                exerciseSession.Records.Add(new ExerciseRecord()
                {
                    Id = Guid.Parse(reader.Current[3].ToString()),
                    EpochTimestamp = double.Parse(reader.Current[4].ToString()),
                    Set = reader.Current[5].ToString(),
                    Reps = (Int64)reader.Current[6],
                    Value = double.Parse(reader.Current[7].ToString()),
                    Unit = reader.Current[8].ToString(),
                    DropSet = (bool)reader.Current[9]
                });
            }
            return exerciseSession;
        }
    }
}
