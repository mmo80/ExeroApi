using System;

namespace Exero.Api.Models
{
    public class ExerciseCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public User User { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
