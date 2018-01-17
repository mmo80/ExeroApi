
namespace Exero.Api
{
    public class ExeroSettings
    {
        public Neo4jSettings Neo4jSettings { get; set; }
    }

    public class Neo4jSettings
    {
        public string Uri { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
