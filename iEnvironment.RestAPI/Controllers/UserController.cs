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


        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(User user)
        {
            var created = await userService.CreateNew(user);
            if (created)
            {
                return new OkResult();
            }
            return new BadRequestResult();
        }

        [HttpGet]
        [Route("getByLogin/{login}")]
        public async Task<ActionResult<User>> GetByLogin([FromRoute] string login)
        {
            var user = await userService.GetByLogin(login);
            if(user == null)
            {
                return new NotFoundResult();
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("edit/{id}")]
        public async Task<ActionResult> EditUser([FromRoute] string id, [FromBody] User user)
        {
            var edited = await userService.EditUser(id, user);
            if (edited)
            {
                return Ok(user);
            }

            return new BadRequestResult();
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginAttempt attempt)
        {
            var user = await userService.Authenticate(attempt);
            if(user == null)
            {
                return new UnauthorizedResult();
            }

            return Ok(user);
        }

    }
}
