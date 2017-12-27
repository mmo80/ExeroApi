using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Models;

namespace Exero.Api.Repositories
{
    public class ExerciseMemoryRepository : IExerciseRepository
    {
        private readonly IList<Exercise> _exercices;

        public ExerciseMemoryRepository()
        {
            _exercices = InMemoryData.Exercices;
        }

        public Task<IEnumerable<Exercise>> GetAll(Guid userId, Guid categoryId)
        {
            return Task.Run(() =>
            {
                return _exercices.Where(x => x.Category.Id == categoryId && x.Category.User.Id == userId);
            });
        }

        public Task<Exercise> Get(Guid userId, Guid id)
        {
            return Task.Run(() =>
            {
                return _exercices.First(x => x.Id == id && x.Category.User.Id == userId);
            });
        }

        public Task<Exercise> Add(Exercise exercise)
        {
            return Task.Run(() =>
            {
                _exercices.Add(exercise);
                return exercise;
            });
        }

        public Task<Exercise> Update(Guid userId, Guid categoryId, Guid id, string name, string note)
        {
            return Task.Run(() =>
            {
                var exercise = _exercices.First(x => x.Category.Id == categoryId && x.Category.User.Id == userId);
                if (!string.IsNullOrEmpty(name)) { exercise.Name = name; }
                if (!string.IsNullOrEmpty(note)) { exercise.Note = note; }
                return exercise;
            });
        }
    }
}
