using System;

namespace Exero.Api.Common
{
    public static class DateTimeExtensionMethods
    {
        // url: https://codereview.stackexchange.com/q/125275
        /// <summary>
        /// Converts a DateTime to the long representation which is the number of seconds since the unix epoch.
        /// </summary>
        /// <param name="dateTime">A DateTime to convert to epoch time.</param>
        /// <returns>The long number of seconds since the unix epoch.</returns>
        public static long ToEpoch(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
