using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Employees;
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

        public EmployeesController(IEmployeesRepo employeesRepo)
        {
            _employeesRepo = employeesRepo;
        }

        // GET /api/v1/employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var c = await _employeesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.Employees>>.SuccessResponse(
                    c,
                    "Employees retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.Employees>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/employees
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Models.Employees employee)
        {
            try
            {
                var newId = await _employeesRepo.AddAsync(employee);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Employee added successfully"
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

        // DELETE /api/v1/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] string id)
        {
            try
            {
                var rowsAffected = await _employeesRepo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Employee deleted successfully"
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

        // PUT /api/v1/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] string id, [FromBody] Models.Employees employee)
        {
            try
            {
                var emp = await _employeesRepo.UpdateAsync(id, employee);
                var response = ApiResponse<Models.Employees>.SuccessResponse(
                    emp,
                    "Employee updated successfully"
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
