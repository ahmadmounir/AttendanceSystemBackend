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
    }
}
