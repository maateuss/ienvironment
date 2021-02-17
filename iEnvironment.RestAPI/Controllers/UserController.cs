using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private UserService userService;
        public UserController()
        {
            userService = new UserService();
        }
        [HttpGet]
        public async Task<User> Get(string UserID)
        {
            return await userService.FindByID(UserID);
        }
    }
}
