using System.Security.Claims;
using AttendanceSystemBackend.Extensions;
using AttendanceSystemBackend.Repositories.Auth;

namespace AttendanceSystemBackend.Services.Authorization
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IAuthRepo _authRepo;

        public UserAuthorizationService(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<bool> IsAdminAsync(ClaimsPrincipal user)
        {
            var roleId = user.GetRoleId();
            if (string.IsNullOrEmpty(roleId))
                return false;

            var roleName = await _authRepo.GetRoleNameByIdAsync(roleId);
            return roleName?.ToLower() == "admin";
        }

        public async Task<string?> GetCurrentUserIdAsync(ClaimsPrincipal user)
        {
            return user.GetUserId();
        }

        public async Task<string?> GetCurrentEmployeeIdAsync(ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _authRepo.GetEmployeeIdByUserIdAsync(userId);
        }

        public async Task<bool> IsUserAuthorizedForEmployeeAsync(ClaimsPrincipal user, string employeeId)
        {
            if (await IsAdminAsync(user))
                return true;

            var currentEmployeeId = await GetCurrentEmployeeIdAsync(user);
            return currentEmployeeId == employeeId;
        }

        public async Task<(bool isAuthorized, string? errorMessage)> ValidateAdminAccessAsync(ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return (false, "Not authorized");

            if (!await IsAdminAsync(user))
                return (false, "Only administrators can perform this action");

            return (true, null);
        }

        public async Task<(bool isAuthorized, string? errorMessage)> ValidateEmployeeAccessAsync(ClaimsPrincipal user, string employeeId)
        {
            var userId = user.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return (false, "Not authorized");

            if (!await IsUserAuthorizedForEmployeeAsync(user, employeeId))
                return (false, "You can only access your own records");

            return (true, null);
        }
    }
}
