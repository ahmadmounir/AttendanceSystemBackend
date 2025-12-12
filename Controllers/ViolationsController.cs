using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Violations;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("violations")]
    [Authorize]
    public class ViolationsController : Controller
    {
        private readonly IViolationsRepo _violationsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public ViolationsController(IViolationsRepo violationsRepo, IUserAuthorizationService authorizationService)
        {
            _violationsRepo = violationsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/violations
        [HttpGet]
        public async Task<IActionResult> GetAllViolations()
        {
            try
            {
                IEnumerable<Violation> items;

                if (await _authorizationService.IsAdminAsync(User))
                {
                    items = await _violationsRepo.GetAllAsync();
                }
                else
                {
                    var employeeId = await _authorizationService.GetCurrentEmployeeIdAsync(User);
                    if (string.IsNullOrEmpty(employeeId))
                    {
                        return NotFound(ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                            "Employee not found", 404));
                    }
                    items = await _violationsRepo.GetByEmployeeIdAsync(employeeId);
                }

                return Ok(ApiResponse<IEnumerable<Violation>>.SuccessResponse(
                    items, "Violations retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/violations/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetViolationsByEmployeeId([FromRoute] string employeeId)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateEmployeeAccessAsync(User, employeeId);
                if (!isAuthorized)
                {
                    return StatusCode(403, ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                        errorMessage ?? "Access denied", 403));
                }

                var items = await _violationsRepo.GetByEmployeeIdAsync(employeeId);
                return Ok(ApiResponse<IEnumerable<Violation>>.SuccessResponse(
                    items, "Violations retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/violations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetViolationById([FromRoute] string id)
        {
            try
            {
                var item = await _violationsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound(ApiResponse<Violation>.ErrorResponse(
                        "Violation not found", 404));
                }

                var (isAuthorized, errorMessage) = await _authorizationService.ValidateEmployeeAccessAsync(User, item.EmployeeId);
                if (!isAuthorized)
                {
                    return StatusCode(403, ApiResponse<Violation>.ErrorResponse(
                        errorMessage ?? "Access denied", 403));
                }

                return Ok(ApiResponse<Violation>.SuccessResponse(
                    item, "Violation retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Violation>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/violations (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddViolation([FromBody] Violation violation)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<string>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var newId = Guid.NewGuid().ToString();
                await _violationsRepo.AddAsync(newId, violation);
                return Ok(ApiResponse<string>.SuccessResponse(
                    newId, "Violation added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/violations/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateViolation([FromRoute] string id, [FromBody] Violation violation)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<int>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var rowsAffected = await _violationsRepo.UpdateAsync(id, violation);
                if (rowsAffected == 0)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Violation not found", 404));
                }

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Violation updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // DELETE /api/v1/violations/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViolation([FromRoute] string id)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<int>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var rowsAffected = await _violationsRepo.DeleteAsync(id);
                if (rowsAffected == 0)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Violation not found", 404));
                }

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Violation deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
