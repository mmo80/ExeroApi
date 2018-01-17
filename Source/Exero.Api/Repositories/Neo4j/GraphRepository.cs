using Microsoft.Extensions.Options;
using Neo4j.Driver.V1;

namespace Exero.Api.Repositories.Neo4j
{
    public interface IGraphRepository
    {
        IDriver Driver { get; set; }
    }


    public class GraphRepository : IGraphRepository
    {
        private Neo4jSettings _neo4jSettings;

        public GraphRepository(IOptions<ExeroSettings> settings)
        {
            _neo4jSettings = settings.Value.Neo4jSettings;

            var url = _neo4jSettings.Uri;
            var user = _neo4jSettings.User;
            var password = _neo4jSettings.Password;

            Driver = GraphDatabase.Driver(url, AuthTokens.Basic(user, password));
        }

        public IDriver Driver { get; set; }
    }
}
