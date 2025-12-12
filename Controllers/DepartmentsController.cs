using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Departments;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("departments")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsRepo _departmentsRepo;

        public DepartmentsController(IDepartmentsRepo departmentsRepo)
        {
            _departmentsRepo = departmentsRepo;
        }

        // GET /api/v1/departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _departmentsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Department>>.SuccessResponse(
                    departments,
                    "Departments retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Department>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/departments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById([FromRoute] string id)
        {
            try
            {
                var department = await _departmentsRepo.GetByIdAsync(id);
                if (department == null)
                {
                    var notFoundResponse = ApiResponse<Department>.ErrorResponse(
                        "Department not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Department>.SuccessResponse(
                    department,
                    "Department retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Department>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/departments
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] Models.Department department)
        {
            try
            {
                var newId = await _departmentsRepo.AddAsync(department);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Department added successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // DELETE /api/v1/departments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] string id)
        {
            try
            {
                var rowsAffected = await _departmentsRepo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Department deleted successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<int>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
