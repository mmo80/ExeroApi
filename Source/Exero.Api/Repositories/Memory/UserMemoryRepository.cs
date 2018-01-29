using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories.Memory
{
    public class UserMemoryRepository : IUserRepository
    {
        private readonly IList<User> _users;

        public UserMemoryRepository()
        {
            _users = InMemoryData.Users;
        }

        public Task<User> Get(Guid id)
        {
            return Task.Run(() =>
            {
                return _users.First(x => x.Id == id);
            });
        }

        public Task<User> FindByEmail(string email)
        {
            return Task.Run(() =>
            {
                return _users.First(x => x.Email == email);
            });
        }

        public Task<User> BlockUser(Guid id, bool block)
        {
            return Task.Run(() =>
            {
                var user = _users.First(x => x.Id == id);
                user.Blocked = block;
                return user;
            });
        }
    }
}
