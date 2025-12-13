using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    /// <summary>
    /// Authentication endpoints for login, token refresh, and logout
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User login - Authenticate with username and password
        /// </summary>
        /// <param name="request">Login credentials (username and password)</param>
        /// <returns>Access token, refresh token (in cookie), user information, and role</returns>
        /// <response code="200">Login successful - Returns access token and user info</response>
        /// <response code="401">Invalid credentials - Username or password incorrect</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/login
        ///     {
        ///         "username": "admin@company.com",
        ///         "password": "123456"
        ///     }
        /// 
        /// Sample response:
        /// 
        ///     {
        ///         "success": true,
        ///         "statusCode": 200,
        ///         "message": "Login successful",
        ///         "data": {
        ///             "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///             "username": "admin@company.com",
        ///             "name": "John Doe",
        ///             "role": "Administrator",
        ///             "expiresAt": "2025-12-13T10:00:00Z"
        ///         }
        ///     }
        /// 
        /// The refresh token is automatically stored in an HTTP-only cookie.
        /// </remarks>
        // POST /api/v1/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    var validationResponse = ApiResponse<LoginResponse>.ErrorResponse(
                        "Username and password are required",
                        400
                    );
                    return BadRequest(validationResponse);
                }

                var (result, refreshToken) = await _authService.LoginAsync(request);

                if (result == null || refreshToken == null)
                {
                    var errorResponse = ApiResponse<LoginResponse>.ErrorResponse(
                        "Invalid Credentials",
                        400
                    );
                    return Unauthorized(errorResponse);
                }

                SetRefreshTokenCookie(refreshToken);

                var response = ApiResponse<LoginResponse>.SuccessResponse(
                    result,
                    "Login successful"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<LoginResponse>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/auth/refresh
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    var validationResponse = ApiResponse<LoginResponse>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(validationResponse);
                }

                var (result, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);

                if (result == null || newRefreshToken == null)
                {
                    var errorResponse = ApiResponse<LoginResponse>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                SetRefreshTokenCookie(newRefreshToken);

                var response = ApiResponse<LoginResponse>.SuccessResponse(
                    result,
                    "Token refreshed successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<LoginResponse>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    await _authService.RevokeTokenAsync(refreshToken);
                }

                Response.Cookies.Delete("refreshToken");

                var response = ApiResponse<bool>.SuccessResponse(
                    true,
                    "Logout successful"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
