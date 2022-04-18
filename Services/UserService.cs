using AuthorizationWithCustomClaim.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationWithCustomClaim.Services
{
    public interface IUserService
    {
        ValueTask<AuthResponse> AuthorizeAsync(LoginModel login);

        ValueTask<AuthResponse> GenerateTokenAsync(User user, DateTime? expiry = default);

        ValueTask<List<User>> GetUsers();

        ValueTask<User> GetUser(string userId);

        ValueTask<bool> CreateUser(RegisterUserModel model);
    }
    public class UserService : IUserService
    {
        public UserManager<User> UserManager { get; }
        public SignInManager<User> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        private readonly JWT _jwt;

        public UserService(UserManager<User> userManager,
            SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager,
            IOptions<JWT> options)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _jwt = options.Value;
        }

        public async ValueTask<AuthResponse> AuthorizeAsync(LoginModel login)
        {
            var user = await UserManager.FindByEmailAsync(login.UserName) ?? await UserManager.FindByNameAsync(login.UserName);

            if (user is null)
            {
                throw new KeyNotFoundException("User account not found.");
            }

            if (await UserManager.CheckPasswordAsync(user, login.Password) == false)
                throw new Exception("Username/Password incorrect");

            return await GenerateTokenAsync(user);
        }

        public async ValueTask<AuthResponse> GenerateTokenAsync(User user, DateTime? expiry = default)
        {
            if (expiry is null)
            {
                expiry = DateTime.UtcNow.AddYears(1);
            }

            var key = Encoding.ASCII.GetBytes(_jwt.SigningKey);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            var userClaims = await UserManager.GetClaimsAsync(user);

            if (userClaims?.Count > 0)
            {
                claims.AddRange(userClaims.Select(claim => new Claim(ClaimTypes.Role, claim.Value)));
            }

            var jwtToken = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                IssuedAt = DateTime.UtcNow,
                Expires = expiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(jwtToken);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                Claims = userClaims.Select(c => c.Value).ToList(),
                UserId = user.Id,
            };
        }

        public async ValueTask<List<User>> GetUsers()
        {
            return await UserManager.Users.ToListAsync();
        }

        public async ValueTask<User> GetUser(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
        }

        public async ValueTask<bool> CreateUser(RegisterUserModel model)
        {
            var user = new User()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                RoleName = model.RoleName,
                PhoneNumber = model.PhoneNumber
            };

            var createUserResult = await UserManager.CreateAsync(user, model.Password);

            return createUserResult.Succeeded;
        }
    }
}
