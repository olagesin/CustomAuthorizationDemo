using AuthorizationWithCustomClaim.Services;

namespace AuthorizationWithCustomClaim.Configuration
{
    public static class Services
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services) => services
            .AddScoped<IUserService, UserService>();
    }
}
