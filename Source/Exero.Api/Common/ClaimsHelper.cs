using System.Linq;
using System.Security.Claims;

namespace Exero.Api.Common
{
    public class ClaimsHelper
    {
        public static string GetUserValue(ClaimsPrincipal user, ValueType type)
        {
            return user.Claims.First(x => x.Type == $"http://localhost:44343/{type.ToString()}").Value;
        }
    }

    public enum ValueType
    {
        email,
        exero_userid
    }
}
