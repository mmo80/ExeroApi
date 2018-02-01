
namespace Exero.Api
{
    public class ExeroSettings
    {
        public Neo4j Neo4j { get; set; }
        public Auth0 Auth0 { get; set; }
    }

    public class Neo4j
    {
        public string Uri { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class Auth0
    {
        public string Domain { get; set; }
        public string Audience { get; set; }
        public string ConnectionDb { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
