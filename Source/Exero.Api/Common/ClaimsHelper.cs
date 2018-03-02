using System.Linq;
using System.Security.Claims;

namespace Exero.Api.Common
{
    public class ClaimsHelper
    {
        public static string GetUserValue(ClaimsPrincipal user, ValueType type)
        {

#if DEBUG
            return user.Claims.First(x => x.Type == $"http://localhost:44343/{type.ToString()}").Value;
#else
            return user.Claims.First(x => x.Type == $"https://api.exeroapp.com/{type.ToString()}").Value;
#endif

        }
    }

    public enum ValueType
    {
        email,
        exero_userid
    }
}
