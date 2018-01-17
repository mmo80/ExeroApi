using System;

namespace Exero.Api.Models
{
    public class ExerciseRecord : BaseId
    {
        public float EpochTimestamp { get; set; }
        public string Set { get; set; }
        public int Reps { get; set; }
        public float Value { get; set; }
        public string Unit { get; set; }
        public bool DropSet { get; set; }

        // from .Net 4.6 and above use DateTimeOffset.Now.ToUnixTimeSeconds() 

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime Datetime => epoch.AddSeconds(EpochTimestamp);
    }
}
