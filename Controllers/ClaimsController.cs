using AuthorizationWithCustomClaim.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthorizationWithCustomClaim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly string[] defaultClaims = { "Create Users"};

        private readonly RoleManager<IdentityRole> RoleManager;

        public ClaimsController(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }

        [HttpGet("view-all-claims")]
        public IActionResult ViewAllClaims()
        {
            return Ok(defaultClaims);
        }


        [HttpPost("add-claims-to-role")]
        public async ValueTask<IActionResult> AddClaimsToRoles(ClaimRequest[] requests)
        {
            foreach (var request in requests)
            {
                var role = await RoleManager.FindByNameAsync(request.RoleName);

                if (role is null)
                {
                    throw new Exception($"Role [{role}] does not exist.");
                }

                var claims = await RoleManager.GetClaimsAsync(role);
                foreach (var claim in request.Claims)
                {
                    if (!claims.Any(c => c.Value == claim))
                    {
                        await RoleManager.AddClaimAsync(role, new Claim(ClaimTypes.Actor, claim));
                    }
                }
            }

            return Ok("Created");
        }
    }
}
