using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories.Memory
{
    public class CategoryMemoryRepository : ICategoryRepository
    {
        private readonly IList<Category> _categories;

        public CategoryMemoryRepository()
        {
            _categories = InMemoryData.Categories;
        }

        public Task<IEnumerable<Category>> GetAll()
        {
            //return _categories.AsEnumerable();
            return Task.Run(() =>
            {
                return _categories.AsEnumerable();//.Where(x => x.User.Id == userId);
            });
        }

        public Task<Category> Get(Guid id)
        {
            return Task.Run(() =>
            {
                return _categories.First(c => c.Id == id); //  && c.User.Id == userId
            });
        }

        public Task<Category> Add(Category category)
        {
            return Task.Run(() =>
            {
                _categories.Add(category);
                return category;
            });
        }

        public Task<Category> Update(Category category)
        {
            return Task.Run(() =>
            {
                var c = _categories.First(x => x.Id == category.Id); //  && c.User.Id == userId
                if (!string.IsNullOrEmpty(category.Name)) { c.Name = category.Name; }
                if (!string.IsNullOrEmpty(category.Note)) { c.Note = category.Note; }
                return c;
            });
        }

        public Task Remove(Guid id)
        {
            var category = _categories.First(c => c.Id == id); //  && c.User.Id == userId
            _categories.Remove(category);

            return Task.CompletedTask;
            //return Task.Run(() =>
            //{
            //    var category = _categories.First(c => c.Id == id && c.User.Id == userId);
            //    _categories.Remove(category);
            //});
        }
    }
}
