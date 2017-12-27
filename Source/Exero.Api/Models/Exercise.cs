using System;

namespace Exero.Api.Models
{
    public class Exercise
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ExerciseCategory Category { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
