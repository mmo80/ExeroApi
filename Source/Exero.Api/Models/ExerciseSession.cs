using System.Collections.Generic;

namespace Exero.Api.Models
{
    public class ExerciseSession : BaseId
    {
        public string Note { get; set; }
        public string ExerciseName { get; set; }
        public IList<ExerciseRecord> Records { get; set; }
    }
}