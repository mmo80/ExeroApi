using System;

namespace Exero.Api.Models
{
    public class ExerciseRecord : BaseId
    {
        public double EpochTimestamp { get; set; }
        public string Set { get; set; }
        public Int64 Reps { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public bool DropSet { get; set; }
        public string Note { get; set; }

        // from .Net 4.6 and above use DateTimeOffset.Now.ToUnixTimeSeconds() 

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime Datetime => epoch.AddSeconds(EpochTimestamp);
    }
}
