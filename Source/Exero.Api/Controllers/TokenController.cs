using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("token")]
    public class TokenController : Controller
    {
        private readonly Auth0 _auth0;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<TokenController> _logger;

        public TokenController(IOptions<ExeroSettings> settings, IUserRepository userRepository, ILogger<TokenController> logger)
        {
            _auth0 = settings.Value.Auth0;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginApi loginApi)
        {
            var request = GetRequest("oauth/token");
            request.AddJsonBody(new
            {
                grant_type = "password",
                username = loginApi.Email,
                password = loginApi.Password,
                realm = _auth0.ConnectionDb,
                client_id = _auth0.ClientId,
                client_secret = _auth0.ClientSecret
            });
            var result = await GetResponse<TokenResultApi>(request);

            _logger.LogDebug($@"Login with following values against auth0.com. 
                Domain: {_auth0.Domain}, Audience: {_auth0.Audience}, ConnectionDb: '{_auth0.ConnectionDb}', ClientId: '{_auth0.ClientId}'.");

            if (!result.IsSuccessful)
            {
                _logger.LogError($"Login against auth0 failed. Reason: {result.Content}.");
                if (result.ErrorException != null)
                {
                    _logger.LogError(result.ErrorException, $"ErrorMessage: {result.ErrorMessage}.");
                }
                return Unauthorized();
            }

            return Ok(result.Data);
        }

        [HttpPost("forgot"), AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgottApi forgottApi)
        {
            var request = GetRequest("dbconnections/change_password");
            request.AddJsonBody(new
            {
                connection = _auth0.ConnectionDb,
                email = forgottApi.Email
            });
            var result = await GetResponse<string>(request);

            if (!result.IsSuccessful)
            {
                _logger.LogError($"ForgotPassword against auth0 failed. Reason: {result.Content}.");
                if (result.ErrorException != null)
                {
                    _logger.LogError(result.ErrorException, $"ErrorMessage: {result.ErrorMessage}.");
                }
                return BadRequest("Failed to request new password.");
            }

            return Ok(result.Content);
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginApi loginApi)
        {
            Guid userId = Guid.NewGuid();

            var request = GetRequest("dbconnections/signup");
            request.AddJsonBody(new
            {
                client_id = _auth0.ClientId,
                email = loginApi.Email,
                password = loginApi.Password,
                connection = _auth0.ConnectionDb,
                user_metadata = new
                {
                    exero_user_id = userId.ToString(),
                    //admin = false
                }
            });
            var result = await GetResponse<SignupResultApi>(request);

            if (!result.IsSuccessful)
            {
                _logger.LogError($"Register against auth0 failed. Reason: {result.Content}.");
                if (result.ErrorException != null)
                {
                    _logger.LogError(result.ErrorException, $"ErrorMessage: {result.ErrorMessage}.");
                }
                return BadRequest("Failed to register.");
            }
            
            var user = new User
            {
                Id = userId,
                Email = loginApi.Email
            };
            await _userRepository.Add(user);
            
            return Ok(result.Data);
        }

        // TODO: Logout?

        private RestRequest GetRequest(string resource)
        {
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("Content-type", "application/json");
            return request;
        }

        private Task<IRestResponse<T>> GetResponse<T>(RestRequest request) where T : class
        {
            var client = new RestClient($"https://{_auth0.Domain}/");
            return client.ExecuteTaskAsync<T>(request);
        }
    }


    public class SignupResultApi
    {
        public string _id { get; set; }
        public string email_verified { get; set; }
        public string email { get; set; }
        public string user_metadata { get; set; }
    }

    public class TokenResultApi
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }

    public class LoginApi
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class ForgottApi
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}