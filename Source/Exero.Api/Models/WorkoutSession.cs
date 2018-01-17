
namespace Exero.Api.Models
{
    public class WorkoutSession : BaseId
    {
        public float StartEpochTimestamp { get; set; }
        public float EndEpochTimestamp { get; set; }
        public string Note { get; set; }
        // ImageUrl
    }
}
