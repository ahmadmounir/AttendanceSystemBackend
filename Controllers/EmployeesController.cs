using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.Employees;
using AttendanceSystemBackend.Repositories.Auth;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("employees")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesRepo _employeesRepo;
        private readonly IAuthRepo _authRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public EmployeesController(IEmployeesRepo employeesRepo, IAuthRepo authRepo, IUserAuthorizationService authorizationService)
        {
            _employeesRepo = employeesRepo;
            _authRepo = authRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/employees (All users can view)
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var c = await _employeesRepo.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<Models.Employees>>.SuccessResponse(
                    c, "Employees retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Models.Employees>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/employees (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeWithAccountDto dto)
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

                var employee = new Models.Employees
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    HireDate = dto.HireDate,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ShiftId = dto.ShiftId,
                    DepartmentId = dto.DepartmentId,
                    JobId = dto.JobId,
                    CountryId = dto.CountryId,
                    IsSystemActive = dto.IsSystemActive
                };

                var employeeId = await _employeesRepo.AddAsync(employee);

                var userAccount = new Models.UserAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployeeId = employeeId,
                    Username = dto.Username,
                    Password = dto.Password,
                    RoleId = dto.RoleId
                };

                await _authRepo.CreateUserAccountAsync(userAccount);

                return Ok(ApiResponse<string>.SuccessResponse(
                    employeeId, "Employee added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // DELETE /api/v1/employees/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] string id)
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

                var rowsAffected = await _employeesRepo.DeleteAsync(id);
                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Employee deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/employees/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] string id, [FromBody] Models.Employees employee)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<Models.Employees>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var emp = await _employeesRepo.UpdateAsync(id, employee);
                return Ok(ApiResponse<Models.Employees>.SuccessResponse(
                    emp, "Employee updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
