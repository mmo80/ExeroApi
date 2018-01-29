using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll(Guid userId);
        Task<Category> Get(Guid userId, Guid id);
        Task<Category> Add(Category category);
        Task<Category> Update(Guid userId, Category category);
        Task Remove(Guid userId, Guid id);
    }
}