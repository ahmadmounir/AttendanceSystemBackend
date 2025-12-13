using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.Employees;
using AttendanceSystemBackend.Repositories.Departments;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using AttendanceSystemBackend.Repositories.OvertimeRequests;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IEmployeesRepo _employeesRepo;
        private readonly IDepartmentsRepo _departmentsRepo;
        private readonly ILeaveRequestsRepo _leaveRequestsRepo;
        private readonly IOvertimeRequestsRepo _overtimeRequestsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public DashboardController(
            IEmployeesRepo employeesRepo,
            IDepartmentsRepo departmentsRepo,
            ILeaveRequestsRepo leaveRequestsRepo,
            IOvertimeRequestsRepo overtimeRequestsRepo,
            IUserAuthorizationService authorizationService)
        {
            _employeesRepo = employeesRepo;
            _departmentsRepo = departmentsRepo;
            _leaveRequestsRepo = leaveRequestsRepo;
            _overtimeRequestsRepo = overtimeRequestsRepo;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Get dashboard statistics - Admin only
        /// </summary>
        /// <returns>Dashboard statistics including pending requests, departments, and employees count</returns>
        /// <response code="200">Dashboard statistics retrieved successfully</response>
        /// <response code="401">Not authorized - User not authenticated</response>
        /// <response code="403">Forbidden - User is not an admin</response>
        /// <remarks>
        /// Sample response:
        /// 
        ///     {
        ///         "success": true,
        ///         "statusCode": 200,
        ///         "message": "Dashboard statistics retrieved successfully",
        ///         "data": {
        ///             "pendingOvertimeRequests": 5,
        ///             "pendingLeaveRequests": 12,
        ///             "totalDepartments": 8,
        ///             "totalEmployees": 150
        ///         }
        ///     }
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<DashboardStatsDto>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var pendingOvertimeCount = await _overtimeRequestsRepo.GetPendingCountAsync();
                var pendingLeaveCount = await _leaveRequestsRepo.GetPendingCountAsync();
                var departmentsCount = await _departmentsRepo.GetCountAsync();
                var employeesCount = await _employeesRepo.GetCountAsync();

                var stats = new DashboardStatsDto
                {
                    PendingOvertimeRequests = pendingOvertimeCount,
                    PendingLeaveRequests = pendingLeaveCount,
                    TotalDepartments = departmentsCount,
                    TotalEmployees = employeesCount
                };

                return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(
                    stats, "Dashboard statistics retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<DashboardStatsDto>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
