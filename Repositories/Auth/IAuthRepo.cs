namespace AttendanceSystemBackend.Repositories.Auth
{
    public interface IAuthRepo
    {
        Task<Models.UserAccount?> GetUserByUsernameAsync(string username);
        Task<Models.UserAccount?> GetUserByIdAsync(string userId);
        Task<string?> GetRoleNameByIdAsync(string roleId);
        Task<string> CreateRefreshTokenAsync(Models.RefreshToken refreshToken);
        Task<Models.RefreshToken?> GetRefreshTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<bool> RevokeAllUserRefreshTokensAsync(string userId);
        Task<string?> GetEmployeeIdByUserIdAsync(string userId);
        Task<string?> GetRoleIdByUserIdAsync(string userId);
        Task<string> CreateUserAccountAsync(Models.UserAccount userAccount);
        Task<string?> GetEmployeeNameByIdAsync(string employeeId);
        Task<Models.UserAccount?> GetUserAccountByEmployeeIdAsync(string employeeId);
        Task<int> UpdateUserAccountAsync(string employeeId, string username, string? password, string roleId);
        Task<int> DeleteUserAccountByEmployeeIdAsync(string employeeId);
    }
}
