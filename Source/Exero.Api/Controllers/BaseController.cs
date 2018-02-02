using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exero.Api.Common;
using Exero.Api.Models;
using Exero.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exero.Api.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUserRepository _userRepository;

        public BaseController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResult> CheckUser()
        {
            var user = await _userRepository.ByEmail(ClaimsHelper.GetUserValue(User, Common.ValueType.email));
            return new UserResult(user);
        }
    }

    public class UserResult
    {
        public UserResult(User user)
        {
            User = user;
        }
        public bool Exist => User != null && User.Id != Guid.Empty;
        public bool NotExist => !Exist;
        public User User { get; set; }
    }
}
