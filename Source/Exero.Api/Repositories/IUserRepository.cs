using System;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(Guid id);
        Task<User> FindByEmail(string email);
        Task SetDisabled(Guid id, bool isDisabled);
    }
}