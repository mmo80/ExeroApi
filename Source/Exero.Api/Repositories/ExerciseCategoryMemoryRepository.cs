using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public class ExerciseCategoryMemoryRepository : IExerciseCategoryRepository
    {
        private IList<ExerciseCategory> _categories;

        public ExerciseCategoryMemoryRepository()
        {
            var user = new User() {Id = Guid.Parse("f93ac602-e896-47ec-b5b5-d11702c033de"), Email = "mmo_80@yahoo.se"};

            _categories = new List<ExerciseCategory>()
            {
                new ExerciseCategory() { Id = Guid.Parse("b6a521ef-a47b-4966-ae02-cb889e8f2cc3"), Name = "Biceps", User = user},
                new ExerciseCategory() { Id = Guid.Parse("d064b338-55e3-4d5d-a33a-10ae6711894a"), Name = "Chest" , User = user}
            };
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

            //return Task.CompletedTask;
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
