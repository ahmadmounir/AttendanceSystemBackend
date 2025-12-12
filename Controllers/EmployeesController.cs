using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Employees;
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
        private readonly IUserAuthorizationService _authorizationService;

        public EmployeesController(IEmployeesRepo employeesRepo, IUserAuthorizationService authorizationService)
        {
            _employeesRepo = employeesRepo;
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
        public async Task<IActionResult> AddEmployee([FromBody] Models.Employees employee)
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

                var newId = await _employeesRepo.AddAsync(employee);
                return Ok(ApiResponse<string>.SuccessResponse(
                    newId, "Employee added successfully"));
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
