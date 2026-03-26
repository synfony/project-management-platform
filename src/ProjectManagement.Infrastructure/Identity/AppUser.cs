using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
