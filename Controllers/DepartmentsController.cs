using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
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

        /// <summary>
        /// Get all departments with employee count - All users can view
        /// </summary>
        /// <returns>List of departments with employee count</returns>
        /// <response code="200">Departments retrieved successfully</response>
        /// <remarks>
        /// Sample response:
        /// 
        ///     [
        ///         {
        ///             "id": "3794CA53-1A31-4D4F-9A73-3FC4865C2CDD",
        ///             "departmentName": "Human Resources (HR)",
        ///             "employeeCount": 15
        ///         },
        ///         {
        ///             "id": "687BD2F9-16DB-4479-B9FD-02D758769AFF",
        ///             "departmentName": "Information Technology (IT)",
        ///             "employeeCount": 42
        ///         }
        ///     ]
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentsRepo.GetAllWithEmployeeCountAsync();
                return Ok(ApiResponse<IEnumerable<DepartmentWithEmployeeCountDto>>.SuccessResponse(
                    departments, "Departments retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<DepartmentWithEmployeeCountDto>>.ErrorResponse(
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
        public async Task<IActionResult> AddDepartment([FromBody] Models.DTOs.DepartmentReqDto department)
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

                var newId = await _departmentsRepo.AddAsync(department.DepartmentName);
                return Ok(ApiResponse<string>.SuccessResponse(
                    newId, "Department added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/departments/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment([FromRoute] string id, [FromBody] Models.DTOs.DepartmentReqDto department)
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

                var rowsAffected = await _departmentsRepo.UpdateAsync(id, department.DepartmentName);
                if (rowsAffected == 0)
                {
                    return NotFound(ApiResponse<int>.ErrorResponse(
                        "Department not found", 404));
                }

                return Ok(ApiResponse<int>.SuccessResponse(
                    rowsAffected, "Department updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse(
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
