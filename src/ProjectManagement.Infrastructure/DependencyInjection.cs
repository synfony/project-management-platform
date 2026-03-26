using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Identity;
using ProjectManagement.Infrastructure.Repositories;
using ProjectManagement.Infrastructure.Services;

namespace ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlite(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, IdentityRole>(opts =>
        {
            opts.Password.RequireDigit = false;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
