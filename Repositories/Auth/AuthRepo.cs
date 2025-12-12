using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Auth
{
    public class AuthRepo : IAuthRepo
    {
        private readonly string _connectionString;

        public AuthRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Models.UserAccount?> GetUserByUsernameAsync(string username)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM UserAccounts WHERE username = @Username";
            var parameters = new { Username = username };
            return await connection.QueryFirstOrDefaultAsync<Models.UserAccount>(sql, parameters);
        }

        public async Task<Models.UserAccount?> GetUserByIdAsync(string userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM UserAccounts WHERE id = @UserId";
            var parameters = new { UserId = userId };
            return await connection.QueryFirstOrDefaultAsync<Models.UserAccount>(sql, parameters);
        }

        public async Task<string?> GetRoleNameByIdAsync(string roleId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT roleName FROM UserRoles WHERE id = @RoleId";
            var parameters = new { RoleId = roleId };
            return await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);
        }

        public async Task<string> CreateRefreshTokenAsync(Models.RefreshToken refreshToken)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO RefreshTokens (id, userId, token, isActive, createdAt, expiresAt) 
                        VALUES (@Id, @UserId, @Token, @IsActive, @CreatedAt, @ExpiresAt)";
            
            var parameters = new
            {
                Id = refreshToken.Id,
                UserId = refreshToken.UserId,
                Token = refreshToken.Token,
                IsActive = refreshToken.IsActive,
                CreatedAt = refreshToken.CreatedAt,
                ExpiresAt = refreshToken.ExpiresAt
            };

            await connection.ExecuteAsync(sql, parameters);
            return refreshToken.Id;
        }

        public async Task<Models.RefreshToken?> GetRefreshTokenAsync(string token)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM RefreshTokens WHERE token = @Token AND isActive = 1";
            var parameters = new { Token = token };
            return await connection.QueryFirstOrDefaultAsync<Models.RefreshToken>(sql, parameters);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE RefreshTokens SET isActive = 0 WHERE token = @Token";
            var parameters = new { Token = token };
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> RevokeAllUserRefreshTokensAsync(string userId)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE RefreshTokens SET isActive = 0 WHERE userId = @UserId";
            var parameters = new { UserId = userId };
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }
    }
}
