using AuthorizationWithCustomClaim.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Reflection;
using AuthorizationWithCustomClaim.Entities;

namespace AuthorizationWithCustomClaim.Utilities
{
    public enum AuthorizationType
    {
        AuthorizeByRoleClaim,
        AuthorizeByUserClaim
    }

    public class Autho
    {
        private readonly string Claim;
        public Autho(string claim)
        {
            Claim = claim;
        }
        private AuthorizationFilterContext AuthorizeByRoleClaims<T>(AuthorizationFilterContext context, T dbContext) where T : IdentityDbContext<User>
        {
            var userProg = typeof(Program).GetTypeInfo().Assembly
                .GetExportedTypes()
                .FirstOrDefault(c => c.IsAbstract == false && c.GetInterfaces().Contains(typeof(T)));

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

        public AuthorizationFilterContext AuthorizeByUserClaims<T>(AuthorizationFilterContext context, T dbContext) where T : IdentityDbContext<User>
        {
            var userProg = typeof(Program).GetTypeInfo().Assembly
            .GetExportedTypes();


            var first = userProg.FirstOrDefault(c => c.BaseType?.Name == "IdentityDbContext`1");

            var properties = first.GetProperties();


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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthChecker : Attribute, IAllowAnonymous, IAuthorizationFilter
    {

        private readonly string Claim;
        private readonly AuthorizationType AuthorizationType;
        private AppDbContext dbContext;


        public CustomAuthChecker(string claim, AuthorizationType authorizationType = AuthorizationType.AuthorizeByRoleClaim)
        {
            Claim = claim;
            AuthorizationType = authorizationType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            dbContext = context.HttpContext
                  .RequestServices
                  .GetService(typeof(AppDbContext)) as AppDbContext;


            if (AuthorizationType == AuthorizationType.AuthorizeByRoleClaim)
            {
                Autho autho = new Autho(Claim);
                autho.AuthorizeByUserClaims(context, dbContext);
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

            return;
        }

        private AuthorizationFilterContext AuthorizeByRoleClaims(AuthorizationFilterContext context)
        {

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
