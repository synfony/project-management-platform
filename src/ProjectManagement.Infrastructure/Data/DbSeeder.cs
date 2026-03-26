using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        const string email = "admin@test.com";
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new AppUser { Email = email, UserName = email, FullName = "Admin User" };
            await userManager.CreateAsync(user, "Admin123");
        }
    }
}
