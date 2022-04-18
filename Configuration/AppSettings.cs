using AuthorizationWithCustomClaim.Entities;

namespace AuthorizationWithCustomClaim.Configuration
{
    public static class IOptionsConfiguration
    {
        public static IServiceCollection ConfigureAppSetting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JWT>(configuration.GetSection("JWT"));

            return services;
        }
    }
}
