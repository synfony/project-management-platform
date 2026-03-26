using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<Result<AuthResultDto>> RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser { Email = dto.Email, UserName = dto.Email, FullName = dto.FullName };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return Result.Failure<AuthResultDto>(string.Join(", ", result.Errors.Select(e => e.Description)));
        return Result.Success(new AuthResultDto
        {
            Token = GenerateToken(user),
            Email = user.Email!,
            FullName = user.FullName
        });
    }

    public async Task<Result<AuthResultDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Result.Failure<AuthResultDto>("Invalid credentials.");
        return Result.Success(new AuthResultDto
        {
            Token = GenerateToken(user),
            Email = user.Email!,
            FullName = user.FullName
        });
    }

    private string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("fullName", user.FullName)
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
