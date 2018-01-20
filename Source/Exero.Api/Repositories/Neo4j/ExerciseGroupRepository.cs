using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class ExerciseGroupRepository : IExerciseGroupRepository
    {
        private readonly IGraphRepository _graphRepository;

        public ExerciseGroupRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<List<ExerciseGroup>> ByCategory(Guid categoryId)
        {
            var list = new List<ExerciseGroup>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (eg:ExerciseGroup)-[r:FOR_CATEGORY]->(c:Category) WHERE c.id = $id
                    RETURN eg.id, eg.name, eg.note",
                    new { id = categoryId.ToString() });

                while (await reader.FetchAsync())
                {
                    list.Add(new ExerciseGroup()
                    {
                        Id = Guid.Parse(reader.Current[0].ToString()),
                        Name = reader.Current[1].ToString(),
                        Note = reader.Current[2].ToString()
                    });
                }
            }
            return list;
        }

        public async Task<ExerciseGroup> Get(Guid id)
        {
            ExerciseGroup item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (eg:ExerciseGroup) WHERE eg.id = $id RETURN eg.id, eg.name, eg.note",
                    new { id = id.ToString() }
                );
                item = await GetExerciseGroup(reader);
            }
            return item;
        }

        public async Task<ExerciseGroup> Add(ExerciseGroup exerciseGroup, Guid categoryId)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (c:Category) WHERE c.id = $categoryId
                    CREATE (eg:ExerciseGroup { id: $id, name: $name, note: $note }),
                    (eg)-[:FOR_CATEGORY]->(c)
                    RETURN eg.id, eg.name, eg.note",
                    new
                    {
                        categoryId = categoryId.ToString(),
                        id = exerciseGroup.Id.ToString(),
                        name = exerciseGroup.Name,
                        note = exerciseGroup.Note
                    }
                );
                exerciseGroup = await GetExerciseGroup(reader);
            }
            return exerciseGroup;
        }


        public async Task<ExerciseGroup> Update(ExerciseGroup exerciseGroup)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (eg:ExerciseGroup) WHERE eg.id = $id 
                    SET eg.name = $name, eg.note = $note 
                    RETURN eg.id, eg.name, eg.note",
                    new { id = exerciseGroup.Id.ToString(), name = exerciseGroup.Name, note = exerciseGroup.Note }
                );
                exerciseGroup = await GetExerciseGroup(reader);
            }
            return exerciseGroup;
        }


        private async Task<ExerciseGroup> GetExerciseGroup(IStatementResultCursor reader)
        {
            ExerciseGroup item = null;
            while (await reader.FetchAsync())
            {
                item = new ExerciseGroup()
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
