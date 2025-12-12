using AttendanceSystemBackend.Models.DTOs;

namespace AttendanceSystemBackend.Services.Auth
{
    public interface IAuthService
    {
        Task<(LoginResponse? response, string? refreshToken)> LoginAsync(LoginRequest request);
        Task<(LoginResponse? response, string? refreshToken)> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        string GenerateAccessToken(string userId, string username, string roleId);
        string GenerateRefreshToken();
    }
}
