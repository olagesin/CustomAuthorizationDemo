using AuthorizationWithCustomClaim.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AuthorizationWithCustomClaim.Utilities
{
    public enum AuthorizationType
    {
        AuthorizeByRoleClaim,
        AuthorizeByUserClaim
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthChecker : Attribute, IAllowAnonymous, IAuthorizationFilter
    {
        private readonly string Claim;
        private AuthorizationType AuthorizationType;
        public CustomAuthChecker(string claim, AuthorizationType authorizationType = AuthorizationType.AuthorizeByRoleClaim)
        {
            Claim = claim;
            AuthorizationType = authorizationType;
        }

        //public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{

        //    if(AuthorizationType == AuthorizationType.AuthorizeByRoleClaim)
        //    {
        //        var result = AuthorizeByRoleClaims(context);
        //        if (result != null)
        //            return;
        //    }
        //    else if(AuthorizationType == AuthorizationType.AuthorizeByUserClaim)
        //    {
        //        var contextResult = AuthorizeByUserClaims(context);
        //        if (contextResult != null)
        //            return;
        //    }

        //    await next();
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AuthorizationType == AuthorizationType.AuthorizeByRoleClaim)
            {
                var result = AuthorizeByRoleClaims(context);
                if (result != null)
                    return;
            }
            else if (AuthorizationType == AuthorizationType.AuthorizeByUserClaim)
            {
                var contextResult = AuthorizeByUserClaims(context);
                if (contextResult != null)
                    return;
            }

            //context.Result = new UnauthorizedResult();
            return;
        }

        private AuthorizationFilterContext AuthorizeByRoleClaims(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext
                  .RequestServices
                  .GetService(typeof(AppDbContext)) as AppDbContext;

            var UserRoleName = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            

            var role = dbContext.Roles.FirstOrDefault(c => c.Name == UserRoleName);
            if (role == null)
            {
                context.Result = new UnauthorizedResult();
                return context;
            }

            var roleContainsClaim = dbContext.RoleClaims.FirstOrDefault(c => c.RoleId == role.Id && c.ClaimValue == Claim);
            if (roleContainsClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return context;
            }

            return null;
        }

        private AuthorizationFilterContext AuthorizeByUserClaims(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext
                  .RequestServices
                  .GetService(typeof(IdentityDbContext)) as AppDbContext;

            var UserRoleName = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (UserRoleName == null)
            {
                context.Result = new UnauthorizedResult();
                return context;
            }

            var userHasClaim = dbContext.UserClaims.FirstOrDefault(c => c.UserId == userId && c.ClaimValue == Claim);
            if (userHasClaim == null)
            {
                context.Result = new ForbidResult();
                return context;
            }

            return null;
        }
    }
}
