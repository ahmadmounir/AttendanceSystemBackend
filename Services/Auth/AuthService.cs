using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.Auth;

namespace AttendanceSystemBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _authRepo;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepo authRepo, IConfiguration configuration)
        {
            _authRepo = authRepo;
            _configuration = configuration;
        }

        public async Task<(LoginResponse? response, string? refreshToken)> LoginAsync(LoginRequest request)
        {
            var user = await _authRepo.GetUserByUsernameAsync(request.Username);
            
            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                return (null, null);
            }

            await _authRepo.RevokeAllUserRefreshTokensAsync(user.Id);

            var roleName = await _authRepo.GetRoleNameByIdAsync(user.RoleId);
            var employeeName = await _authRepo.GetEmployeeNameByIdAsync(user.EmployeeId);

            var accessToken = GenerateAccessToken(user.Id, user.Username, user.RoleId);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntity = new Models.RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Token = refreshToken,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshTokenExpiry
            };

            await _authRepo.CreateRefreshTokenAsync(refreshTokenEntity);

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                Username = user.Username,
                Name = employeeName ?? "Unknown",
                Role = roleName ?? "Unknown",
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"))
            };

            return (response, refreshToken);
        }

        public async Task<(LoginResponse? response, string? refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _authRepo.GetRefreshTokenAsync(refreshToken);
            
            if (storedToken == null || !storedToken.IsActive || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return (null, null);
            }

            var user = await _authRepo.GetUserByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return (null, null);
            }

            await _authRepo.RevokeRefreshTokenAsync(refreshToken);

            var roleName = await _authRepo.GetRoleNameByIdAsync(user.RoleId);
            var employeeName = await _authRepo.GetEmployeeNameByIdAsync(user.EmployeeId);

            var newAccessToken = GenerateAccessToken(user.Id, user.Username, user.RoleId);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var newRefreshTokenEntity = new Models.RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Token = newRefreshToken,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = newRefreshTokenExpiry
            };

            await _authRepo.CreateRefreshTokenAsync(newRefreshTokenEntity);

            var response = new LoginResponse
            {
                AccessToken = newAccessToken,
                Username = user.Username,
                Name = employeeName ?? "Unknown",
                Role = roleName ?? "Unknown",
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"))
            };

            return (response, newRefreshToken);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            return await _authRepo.RevokeRefreshTokenAsync(refreshToken);
        }

        public string GenerateAccessToken(string userId, string username, string roleId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, roleId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            return password == storedPassword;
        }
    }
}
