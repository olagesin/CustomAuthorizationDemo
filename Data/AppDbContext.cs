using AuthorizationWithCustomClaim.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationWithCustomClaim.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasIndex(c => c.Email).IsUnique(true);
            builder.Entity<User>().HasIndex(c => c.PhoneNumber).IsUnique(true);

            base.OnModelCreating(builder);
        }
    }
}
