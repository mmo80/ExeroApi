using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> Get(Guid id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task Remove(Guid id);
    }
}