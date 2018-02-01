using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Exero.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/token")]
    public class TokenController : Controller
    {
        private readonly Auth0 _auth0;
        private readonly IUserRepository _userRepository;

        public TokenController(IOptions<ExeroSettings> settings, IUserRepository userRepository)
        {
            _auth0 = settings.Value.Auth0;
            _userRepository = userRepository;
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

            return Ok(result.Content);
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginApi loginApi)
        {
            var request = GetRequest("dbconnections/signup");
            request.AddJsonBody(new
            {
                client_id = _auth0.ClientId,
                email = loginApi.Email,
                password = loginApi.Password,
                connection = _auth0.ConnectionDb,
                user_metadata = new
                {
                    exero_user_id = "exero", // TODO: userId Guid?
                    admin = false
                }
            });
            var result = await GetResponse<SignupResultApi>(request);
            if (result.ErrorException != null)
            {
                return BadRequest($"Auth0 responded: {result.Content}");
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = loginApi.Email
            };
            await _userRepository.Add(user);
            
            return Ok(result.Data);
        }


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

        // TODO: Logout?
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