using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IGraphRepository _graphRepository;

        public CategoryRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<IEnumerable<Category>> GetAll(Guid userId)
        {
            var list = new List<Category>();
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (n:Category) RETURN n.id, n.name, n.note");

                while (await reader.FetchAsync())
                {
                    list.Add(new Category()
                    {
                        Id = Guid.Parse(reader.Current[0].ToString()),
                        Name = reader.Current[1].ToString(),
                        Note = reader.Current[2]?.ToString()
                    });
                }
            }
            return list;
        }

        public async Task<Category> Get(Guid userId, Guid id)
        {
            Category category;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (c:Category { id: $id }) RETURN c.id, c.name, c.note",
                    new { id = id.ToString() }
                );
                category = await GetCategory(reader);
            }

            return category;
        }

        public async Task<Category> Add(Category category)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "CREATE (c:Category { id: $id, name: $name }) RETURN c.id, c.name, c.note",
                    new { id = category.Id.ToString(), name = category.Name }
                );
                category = await GetCategory(reader);
            }
            return category;
        }

        public async Task<Category> Update(Guid userId, Category category)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (c:Category { id: $id }) SET c.name = $name, c.note = $note 
                    RETURN c.id, c.name, c.note",
                    new { id = category.Id.ToString(), name = category.Name, note = category.Note }
                );
                category = await GetCategory(reader);
            }
            return category;
        }

        public async Task Remove(Guid userId, Guid id)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                // Deletes node and all relationships to it.
                await session.RunAsync(
                    @"OPTIONAL MATCH (c:Category { id: $id })<-[r]-() 
                    DELETE r, c",
                    new { id = id.ToString() }
                );
            }
        }


        private async Task<Category> GetCategory(IStatementResultCursor reader)
        {
            Category item = null;
            while (await reader.FetchAsync())
            {
                item = new Category()
                {
                    Id = Guid.Parse(reader.Current[0].ToString()),
                    Name = reader.Current[1].ToString(),
                    Note = reader.Current[2]?.ToString()
                };
            }
            return item;
        }
    }
}
