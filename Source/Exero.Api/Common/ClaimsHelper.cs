using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        userId
    }
}
