﻿using AuthorizationWithCustomClaim.Data;
using AuthorizationWithCustomClaim.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationWithCustomClaim.Configuration
{
    public static class DefaultIdentityConfiguration
    {
        public static IdentityBuilder ConfigureDefaultIdentity(this IServiceCollection services) =>
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
}
