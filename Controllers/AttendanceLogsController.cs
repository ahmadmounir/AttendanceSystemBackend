using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.AttendanceLogs;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("attendancelogs")]
    [Authorize]
    public class AttendanceLogsController : Controller
    {
        private readonly IAttendanceLogsRepo _attendanceLogsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public AttendanceLogsController(IAttendanceLogsRepo attendanceLogsRepo, IUserAuthorizationService authorizationService)
        {
            _attendanceLogsRepo = attendanceLogsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/attendancelogs
        [HttpGet]
        public async Task<IActionResult> GetAllAttendanceLogs()
        {
            try
            {
                IEnumerable<Models.AttendanceLog> items;

                if (await _authorizationService.IsAdminAsync(User))
                {
                    items = await _attendanceLogsRepo.GetAllAsync();
                }
                else
                {
                    var employeeId = await _authorizationService.GetCurrentEmployeeIdAsync(User);
                    if (string.IsNullOrEmpty(employeeId))
                    {
                        return NotFound(ApiResponse<IEnumerable<Models.AttendanceLog>>.ErrorResponse(
                            "Employee not found", 404));
                    }
                    items = await _attendanceLogsRepo.GetByEmployeeIdAsync(employeeId);
                }

                return Ok(ApiResponse<IEnumerable<Models.AttendanceLog>>.SuccessResponse(
                    items, "Attendance logs retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Models.AttendanceLog>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/attendancelogs/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceLogById([FromRoute] string id)
        {
            try
            {
                var item = await _attendanceLogsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound(ApiResponse<Models.AttendanceLog>.ErrorResponse(
                        "Attendance log not found", 404));
                }

                var (isAuthorized, errorMessage) = await _authorizationService.ValidateEmployeeAccessAsync(User, item.EmployeeId);
                if (!isAuthorized)
                {
                    return StatusCode(403, ApiResponse<Models.AttendanceLog>.ErrorResponse(
                        errorMessage ?? "Access denied", 403));
                }

                return Ok(ApiResponse<Models.AttendanceLog>.SuccessResponse(
                    item, "Attendance log retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Models.AttendanceLog>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/attendancelogs (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddAttendanceLog([FromBody] Models.AttendanceLog attendanceLog)
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

                var newId = await _attendanceLogsRepo.AddAsync(attendanceLog);
                return Ok(ApiResponse<string>.SuccessResponse(
                    newId, "Attendance log added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/attendancelogs/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendanceLog([FromRoute] string id, [FromBody] Models.AttendanceLog attendanceLog)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<Models.AttendanceLog>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var updated = await _attendanceLogsRepo.UpdateAsync(id, attendanceLog);
                return Ok(ApiResponse<Models.AttendanceLog>.SuccessResponse(
                    updated, "Attendance log updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
