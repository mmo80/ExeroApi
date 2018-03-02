using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NLog;
using NLog.Web;

namespace Exero.Api.Middleware
{
    // Url: https://stackoverflow.com/a/38935583
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static Logger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            _logger.Error(exception);

#if DEBUG
            var result = JsonConvert.SerializeObject(new { error = exception.ToString() });
#else
            var result = JsonConvert.SerializeObject(new { error = "An error occured." });
#endif

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
