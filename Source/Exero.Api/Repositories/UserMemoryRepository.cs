using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public class UserMemoryRepository : IUserRepository
    {
        private IList<User> _users;

        public UserMemoryRepository()
        {
            _users = new List<User>()
            {
                new User() { Id = Guid.Parse("f93ac602-e896-47ec-b5b5-d11702c033de"), Email = "mmo_80@yahoo.se" }
            };
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

        public Task SetDisabled(Guid id, bool isDisabled)
        {
            return Task.Run(() =>
            {
                var user = _users.First(x => x.Id == id);
                user.Disabled = isDisabled;
            });
        }
    }
}
