using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public class ExerciseCategoryMemoryRepository : IExerciseCategoryRepository
    {
        private readonly IList<ExerciseCategory> _categories;

        public ExerciseCategoryMemoryRepository()
        {
            _categories = InMemoryData.Categories;
        }

        public Task<IEnumerable<ExerciseCategory>> GetAll(Guid userId)
        {
            return Task.Run(() =>
            {
                return _categories.Where(x => x.User.Id == userId);
            });
        }

        public Task<ExerciseCategory> Get(Guid userId, Guid id)
        {
            return Task.Run(() =>
            {
                return _categories.First(c => c.Id == id && c.User.Id == userId);
            });
        }

        public Task<ExerciseCategory> Add(ExerciseCategory exerciseCategory)
        {
            return Task.Run(() =>
            {
                _categories.Add(exerciseCategory);
                return exerciseCategory;
            });
        }

        public Task<ExerciseCategory> Update(Guid userId, Guid id, string name, string note)
        {
            return Task.Run(() =>
            {
                var category = _categories.First(c => c.Id == id && c.User.Id == userId);
                if (!string.IsNullOrEmpty(name)) { category.Name = name; }
                if (!string.IsNullOrEmpty(note)) { category.Note = note; }
                return category;
            });
        }

        public Task Remove(Guid userId, Guid id)
        {
            var category = _categories.First(c => c.Id == id && c.User.Id == userId);
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
