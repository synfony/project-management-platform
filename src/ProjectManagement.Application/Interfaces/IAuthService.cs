using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResultDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResultDto>> LoginAsync(LoginDto dto);
}
