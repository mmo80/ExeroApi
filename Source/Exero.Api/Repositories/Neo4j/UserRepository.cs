using System;
using System.Threading.Tasks;
using Exero.Api.Models;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public class UserRepository: IUserRepository
    {
        private readonly IGraphRepository _graphRepository;

        public UserRepository(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public async Task<User> Get(Guid id)
        {
            User item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (u:User { id: $id }) RETURN u.id, u.email, u.blocked",
                    new { id = id.ToString() }
                );
                item = await GetUser(reader);
            }
            return item;
        }

        public async Task<User> ByEmail(string email)
        {
            User item;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    "MATCH (u:User { email: $email }) RETURN u.id, u.email, u.blocked",
                    new { email = email }
                );
                item = await GetUser(reader);
            }
            return item;
        }

        public async Task<User> BlockUser(Guid id, bool block)
        {
            User user;
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"MATCH (u:User { id: $id })
                    SET u.blocked = $blocked
                    RETURN u.id, u.email, u.blocked",
                    new { id = id.ToString(), blocked = block }
                );
                user = await GetUser(reader);
            }
            return user;
        }

        public async Task<User> Add(User user)
        {
            using (var session = _graphRepository.Driver.Session())
            {
                var reader = await session.RunAsync(
                    @"CREATE (u:User { id: $id, email: $email })
                    RETURN u.id, u.email, u.blocked",
                    new
                    {
                        id = user.Id.ToString(),
                        email = user.Email
                    }
                );
                user = await GetUser(reader);
            }
            return user;
        }

        private async Task<User> GetUser(IStatementResultCursor reader)
        {
            User item = null;
            while (await reader.FetchAsync())
            {
                item = new User()
                {
                    Id = Guid.Parse(reader.Current[0].ToString()),
                    Email = reader.Current[1].ToString(),
                    Blocked = (bool?) reader.Current[2] ?? false
                };
            }
            return item;
        }
    }
}
