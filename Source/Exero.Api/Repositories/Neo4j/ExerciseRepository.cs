using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly IGraphRepository _graphRepository;

        public ExerciseRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<List<Exercise>> ByGroup(Guid exerciseGroupId)
        {
            var list = new List<Exercise>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (e:Exercise)-[r:FOR_EXERCISE_GROUP]->(eg:ExerciseGroup) WHERE eg.id = $id
                    RETURN e.id, e.name, e.note",
                    new { id = exerciseGroupId.ToString() });

                while (await reader.FetchAsync())
                {
                    list.Add(new Exercise()
                    {
                        Id = Guid.Parse(reader.Current[0].ToString()),
                        Name = reader.Current[1].ToString(),
                        Note = reader.Current[2].ToString()
                    });
                }
            }
            return list;
        }

        public async Task<Exercise> Get(Guid id)
        {
            Exercise item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (e:Exercise) WHERE e.id = $id RETURN e.id, e.name, e.note",
                    new { id = id.ToString() }
                );
                item = await GetExercise(reader);
            }
            return item;
        }

        public async Task<Exercise> Add(Exercise exercise, Guid exerciseGroupId)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (eg:ExerciseGroup) WHERE eg.id = $exerciseGroupId
                    CREATE (e:Exercise { id: $id, name: $name, note: $note }),
                    (e)-[:FOR_EXERCISE_GROUP]->(eg)
                    RETURN e.id, e.name, e.note",
                    new
                    {
                        exerciseGroupId = exerciseGroupId.ToString(),
                        id = exercise.Id.ToString(),
                        name = exercise.Name,
                        note = exercise.Note
                    }
                );
                exercise = await GetExercise(reader);
            }
            return exercise;
        }

        // TODO: Add Exercse to User?

        public async Task<Exercise> Update(Exercise exercise)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (e:Exercise) WHERE e.id = $id 
                    SET e.name = $name, e.note = $note 
                    RETURN e.id, e.name, e.note",
                    new { id = exercise.Id.ToString(), name = exercise.Name, note = exercise.Note }
                );
                exercise = await GetExercise(reader);
            }
            return exercise;
        }


        private async Task<Exercise> GetExercise(IStatementResultCursor reader)
        {
            Exercise item = null;
            while (await reader.FetchAsync())
            {
                item = new Exercise()
                {
                    Id = Guid.Parse(reader.Current[0].ToString()),
                    Name = reader.Current[1].ToString(),
                    Note = reader.Current[2].ToString()
                };
            }
            return item;
        }
    }
}
