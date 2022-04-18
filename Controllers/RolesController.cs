using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationWithCustomClaim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> RoleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }


        [HttpPost("create-role")]
        public async ValueTask<IActionResult> CreateRoleAsync(string roleName)
        {
            try
            {
                if (await RoleManager.RoleExistsAsync(roleName))
                {
                    throw new Exception("Role already exists.");
                }

                var result = await RoleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    return Ok("Role created successfully");
                }

                throw new Exception(string.Join(",", result.Errors.Select(c => c.Description)));
            }
            catch
            {
                throw;
            }
        }


        [HttpDelete("delete-role")]
        public async ValueTask<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                if (await RoleManager.RoleExistsAsync(roleName) is false)
                {
                    throw new Exception("Role does not exist.");
                }

                var result = await RoleManager.DeleteAsync(await RoleManager.FindByNameAsync(roleName));

                if (result.Succeeded)
                {
                    return Ok("Role deleted successfully");
                }

                throw new Exception(string.Join(",", result.Errors.Select(c => c.Description)));
            }
            catch
            {
                throw;
            }
        }


        [HttpGet("get-all-roles")]
        public async ValueTask<IActionResult> ViewAllRoles()
        {
            try
            {
                var roles = await RoleManager.Roles.Select(c => c.Name).ToListAsync();

                return Ok(roles);
            }
            catch
            {
                throw;
            }
        }
    }
}
