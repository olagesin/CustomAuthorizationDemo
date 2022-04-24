using AuthorizationWithCustomClaim.Entities;
using AuthorizationWithCustomClaim.Services;
using AuthorizationWithCustomClaim.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationWithCustomClaim.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }


        [HttpPost("create-user")]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                var result = await userService.CreateUser(model);

                if (result)
                    return Ok("Created");
                return BadRequest();
            }
        }


        [HttpGet("get-user")]
        [CustomAuthChecker("Get User",AuthorizationType.AuthorizeByRoleClaim)]
        public async Task<IActionResult> GetUser(string userId)
        {
            return Ok(await userService.GetUser(userId));
        }


        [AllowAnonymous]
        [HttpGet("get-users")]
        [CustomAuthChecker("List Users", AuthorizationType.AuthorizeByRoleClaim)]
        public async Task<IActionResult> ListUsers()
        {
            return Ok(await userService.GetUsers());
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            return Ok(await userService.AuthorizeAsync(loginModel));
        }
    }
}
