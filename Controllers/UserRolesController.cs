using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.UserRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("userroles")]
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IUserRolesRepo _userRolesRepo;

        public UserRolesController(IUserRolesRepo userRolesRepo)
        {
            _userRolesRepo = userRolesRepo;
        }

        // GET /api/v1/userroles
        [HttpGet]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var userRoles = await _userRolesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<UserRole>>.SuccessResponse(
                    userRoles,
                    "User roles retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<UserRole>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/userroles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRoleById([FromRoute] string id)
        {
            try
            {
                var userRole = await _userRolesRepo.GetByIdAsync(id);
                if (userRole == null)
                {
                    var notFoundResponse = ApiResponse<UserRole>.ErrorResponse(
                        "User role not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<UserRole>.SuccessResponse(
                    userRole,
                    "User role retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<UserRole>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
