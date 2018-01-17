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

        public Task<IEnumerable<Category>> GetAll(Guid userId)
        {
            //return _categories.AsEnumerable();
            return Task.Run(() =>
            {
                return _categories.AsEnumerable();//.Where(x => x.User.Id == userId);
            });
        }

        public Task<Category> Get(Guid userId, Guid id)
        {
            return Task.Run(() =>
            {
                return _categories.First(c => c.Id == id); //  && c.User.Id == userId
            });
        }

        public Task<Category> Add(Category exerciseCategory)
        {
            return Task.Run(() =>
            {
                _categories.Add(exerciseCategory);
                return exerciseCategory;
            });
        }

        public Task<Category> Update(Guid userId, Guid id, string name, string note)
        {
            return Task.Run(() =>
            {
                var category = _categories.First(c => c.Id == id); //  && c.User.Id == userId
                if (!string.IsNullOrEmpty(name)) { category.Name = name; }
                if (!string.IsNullOrEmpty(note)) { category.Note = note; }
                return category;
            });
        }

        public Task Remove(Guid userId, Guid id)
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
