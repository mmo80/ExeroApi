using System;

namespace Exero.Api.Models
{
    public class WorkoutSession : BaseId
    {
        public double StartEpochTimestamp { get; set; }
        public double EndEpochTimestamp { get; set; }
        public string Note { get; set; }
        // ImageUrl

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime StartDatetime => epoch.AddSeconds(StartEpochTimestamp);
        public DateTime EndDatetime => epoch.AddSeconds(EndEpochTimestamp);
    }
}
