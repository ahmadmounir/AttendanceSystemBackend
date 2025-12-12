using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.EmployeeShifts;
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

        public EmployeeShiftsController(IEmployeeShiftsRepo employeeShiftsRepo)
        {
            _employeeShiftsRepo = employeeShiftsRepo;
        }

        // GET /api/v1/employeeshifts
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeShifts()
        {
            try
            {
                var items = await _employeeShiftsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<EmployeeShift>>.SuccessResponse(
                    items,
                    "Employee shifts retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<EmployeeShift>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/employeeshifts/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeShiftsByEmployeeId([FromRoute] string employeeId)
        {
            try
            {
                var items = await _employeeShiftsRepo.GetByEmployeeIdAsync(employeeId);
                var response = ApiResponse<IEnumerable<EmployeeShift>>.SuccessResponse(
                    items,
                    "Employee shifts retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<EmployeeShift>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/employeeshifts/{employeeId}/{startDate}
        [HttpGet("{employeeId}/{startDate}")]
        public async Task<IActionResult> GetEmployeeShift([FromRoute] string employeeId, [FromRoute] DateOnly startDate)
        {
            try
            {
                var item = await _employeeShiftsRepo.GetByEmployeeAndStartDateAsync(employeeId, startDate);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<EmployeeShift>.ErrorResponse(
                        "Employee shift not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<EmployeeShift>.SuccessResponse(
                    item,
                    "Employee shift retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeShift>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/employeeshifts
        [HttpPost]
        public async Task<IActionResult> AddEmployeeShift([FromBody] EmployeeShift employeeShift)
        {
            try
            {
                await _employeeShiftsRepo.AddAsync(employeeShift);
                var response = ApiResponse<EmployeeShift>.SuccessResponse(
                    employeeShift,
                    "Employee shift added successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeShift>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // PUT /api/v1/employeeshifts/{employeeId}/{startDate}
        [HttpPut("{employeeId}/{startDate}")]
        public async Task<IActionResult> UpdateEmployeeShift([FromRoute] string employeeId, [FromRoute] DateOnly startDate, [FromBody] EmployeeShift employeeShift)
        {
            try
            {
                var rowsAffected = await _employeeShiftsRepo.UpdateAsync(employeeId, startDate, employeeShift);
                if (rowsAffected == 0)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Employee shift not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Employee shift updated successfully"
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

        // DELETE /api/v1/employeeshifts/{employeeId}/{startDate}
        [HttpDelete("{employeeId}/{startDate}")]
        public async Task<IActionResult> DeleteEmployeeShift([FromRoute] string employeeId, [FromRoute] DateOnly startDate)
        {
            try
            {
                var rowsAffected = await _employeeShiftsRepo.DeleteAsync(employeeId, startDate);
                if (rowsAffected == 0)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Employee shift not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Employee shift deleted successfully"
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
