using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Departments;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("departments")]
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsRepo _departmentsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public DepartmentsController(IDepartmentsRepo departmentsRepo, IUserAuthorizationService authorizationService)
        {
            _departmentsRepo = departmentsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/departments (All users can view)
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentsRepo.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<Department>>.SuccessResponse(
                    departments, "Departments retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Department>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // GET /api/v1/departments/{id} (All users can view)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById([FromRoute] string id)
        {
            try
            {
                var department = await _departmentsRepo.GetByIdAsync(id);
                if (department == null)
                {
                    return NotFound(ApiResponse<Department>.ErrorResponse(
                        "Department not found", 404));
                }

                return Ok(ApiResponse<Department>.SuccessResponse(
                    department, "Department retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Department>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/departments (Admin only)
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] Models.Department department)
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
                await _departmentsRepo.AddAsync(newId, department);
                return Ok(ApiResponse<string>.SuccessResponse(
                    newId, "Department added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // DELETE /api/v1/departments/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] string id)
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

                var rowsAffected = await _departmentsRepo.DeleteAsync(id);
                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Department deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }
}
