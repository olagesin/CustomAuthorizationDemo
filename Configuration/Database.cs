using AuthorizationWithCustomClaim.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationWithCustomClaim.Configuration
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
