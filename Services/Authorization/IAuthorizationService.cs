using System.Security.Claims;

namespace AttendanceSystemBackend.Services.Authorization
{
    public interface IUserAuthorizationService
    {
        Task<bool> IsAdminAsync(ClaimsPrincipal user);
        Task<string?> GetCurrentUserIdAsync(ClaimsPrincipal user);
        Task<string?> GetCurrentEmployeeIdAsync(ClaimsPrincipal user);
        Task<bool> IsUserAuthorizedForEmployeeAsync(ClaimsPrincipal user, string employeeId);
        Task<(bool isAuthorized, string? errorMessage)> ValidateAdminAccessAsync(ClaimsPrincipal user);
        Task<(bool isAuthorized, string? errorMessage)> ValidateEmployeeAccessAsync(ClaimsPrincipal user, string employeeId);
    }
}
