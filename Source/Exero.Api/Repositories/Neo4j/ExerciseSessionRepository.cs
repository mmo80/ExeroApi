using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

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
    }
}
