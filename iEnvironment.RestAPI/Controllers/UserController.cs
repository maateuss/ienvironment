using iEnvironment.Domain.Models;
using iEnvironment.RestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace iEnvironment.RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private UserService userService;
        private CryptoService cryptoService;
        public UserController()
        {
            userService = new UserService();
            cryptoService = new CryptoService();
        }
        [HttpGet]
        [Authorize]
        public async Task<User> Get(string UserID)
        {
            return await userService.FindByID(UserID);
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody]User user)
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
        [Authorize]
        public async Task<ActionResult<User>> GetByLogin([FromRoute] string login)
        {
            var user = await userService.GetByLogin(login);
            if (user == null)
            {
                return new NotFoundResult();
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("edit/{userid}")]
        public async Task<ActionResult> EditUser([FromRoute] string userid, [FromBody] User userToUpdate)
        {

            if (userToUpdate == null) return new UnprocessableEntityObjectResult("user to update is null!");
            try
            {

                var edited = await userService.EditUser(userid, userToUpdate);
                if (edited)
                {
                    return Ok(userToUpdate);
                }

                return new ConflictObjectResult("impossible to update user");
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("edit/{userid}")]
        public async Task<ActionResult> PutEditUser([FromRoute] string userid, [FromBody] User userToUpdate)
        {

            if (userToUpdate == null) return new UnprocessableEntityObjectResult("user to update is null!");
            try
            {

                var edited = await userService.EditUser(userid, userToUpdate);
                if (edited)
                {
                    return Ok(JsonConvert.SerializeObject(userToUpdate));
                }

                return new ConflictObjectResult("impossible to update user");
            }
            catch
            {
                return new BadRequestResult();
            }
        }


        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginAttempt attempt)
        {
            var user = await userService.Authenticate(attempt);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            var token = cryptoService.GenerateJWT(user);
            var refreshToken = await cryptoService.GenerateRefreshToken(user);
            return Ok(new { user, token, refreshToken= refreshToken.Value });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("getallusers")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(JsonConvert.SerializeObject(await userService.FindAll()));
        }

        [HttpGet]
        [Authorize]
        [Route("me")]
        public async Task<ActionResult> Me()
        {
            try { 
                var id = User.Claims.FirstOrDefault(x => x.Type == "userid").Value;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var user = await userService.FindByID(id);
                    var newToken = cryptoService.GenerateJWT(user);
                    var refreshToken = await cryptoService.GenerateRefreshToken(user);

                    return Ok(new { user, token = newToken, refreshToken = refreshToken.Value });
                }
            }
            catch (Exception ex){
                return new UnauthorizedObjectResult(ex.Message);
            }

            return new UnauthorizedResult();

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Refresh")]
        public async Task<ActionResult> Refresh([FromBody] Token refresh)
        {
            if (refresh.RefreshToken != null)
            {
                var token = await cryptoService.RetrieveRefreshToken(refresh.RefreshToken);
                if (token.IsValid())
                {
                    var user = await userService.FindByID(token.UserID);
                    var newToken = cryptoService.GenerateJWT(user);
                    return Ok(new { token = newToken, refreshToken = refresh.RefreshToken });
                }
            }


            return new BadRequestResult();
        }
    }
}
