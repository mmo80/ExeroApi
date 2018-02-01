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
        private readonly Api.Neo4j _neo4j;

        public GraphRepository(IOptions<ExeroSettings> settings)
        {
            _neo4j = settings.Value.Neo4j;

            var url = _neo4j.Uri;
            var user = _neo4j.User;
            var password = _neo4j.Password;

            Driver = GraphDatabase.Driver(url, AuthTokens.Basic(user, password));
        }

        public IDriver Driver { get; set; }
    }
}
