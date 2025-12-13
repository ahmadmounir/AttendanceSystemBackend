using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.EmployeeShifts;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("employeeshifts")]
    [Authorize]
    public class EmployeeShiftsController : Controller
    {
        private readonly IEmployeeShiftsRepo _employeeShiftsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public EmployeeShiftsController(IEmployeeShiftsRepo employeeShiftsRepo, IUserAuthorizationService authorizationService)
        {
            _employeeShiftsRepo = employeeShiftsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/employeeshifts (All users can view)
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeShifts()
        {
            try
            {
                var items = await _employeeShiftsRepo.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<EmployeeShift>>.SuccessResponse(
                    items, "Employee shifts retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<EmployeeShift>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/employeeshifts/employee/{employeeId} (All users can view)
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeShiftsByEmployeeId([FromRoute] string employeeId)
        {
            try
            {
                var items = await _employeeShiftsRepo.GetByEmployeeIdAsync(employeeId);
                return Ok(ApiResponse<IEnumerable<EmployeeShift>>.SuccessResponse(
                    items, "Employee shifts retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<EmployeeShift>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/employeeshifts/{employeeId}/{startDate} (All users can view)
        [HttpGet("{employeeId}/{startDate}")]
        public async Task<IActionResult> GetEmployeeShift([FromRoute] string employeeId, [FromRoute] DateTime startDate)
        {
            try
            {
                var item = await _employeeShiftsRepo.GetByEmployeeAndStartDateAsync(employeeId, startDate);
                if (item == null)
                {
                    return NotFound(ApiResponse<EmployeeShift>.ErrorResponse(
                        "Employee shift not found", 404));
                }

                return Ok(ApiResponse<EmployeeShift>.SuccessResponse(
                    item, "Employee shift retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EmployeeShift>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/employeeshifts (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddEmployeeShift([FromBody] EmployeeShift employeeShift)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<EmployeeShift>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                await _employeeShiftsRepo.AddAsync(employeeShift);
                return Ok(ApiResponse<EmployeeShift>.SuccessResponse(
                    employeeShift, "Employee shift added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EmployeeShift>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/employeeshifts/{employeeId}/{startDate} (Admin only)
        [HttpPut("{employeeId}/{startDate}")]
        public async Task<IActionResult> UpdateEmployeeShift([FromRoute] string employeeId, [FromRoute] DateTime startDate, [FromBody] EmployeeShift employeeShift)
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

                var rowsAffected = await _employeeShiftsRepo.UpdateAsync(employeeId, startDate, employeeShift);
                if (rowsAffected == 0)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Employee shift not found", 404));
                }

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Employee shift updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // DELETE /api/v1/employeeshifts/{employeeId}/{startDate} (Admin only)
        [HttpDelete("{employeeId}/{startDate}")]
        public async Task<IActionResult> DeleteEmployeeShift([FromRoute] string employeeId, [FromRoute] DateTime startDate)
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

                var rowsAffected = await _employeeShiftsRepo.DeleteAsync(employeeId, startDate);
                if (rowsAffected == 0)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Employee shift not found", 404));
                }

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Employee shift deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
