using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Violations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("violations")]
    [Authorize]
    public class ViolationsController : Controller
    {
        private readonly IViolationsRepo _violationsRepo;

        public ViolationsController(IViolationsRepo violationsRepo)
        {
            _violationsRepo = violationsRepo;
        }

        // GET /api/v1/violations
        [HttpGet]
        public async Task<IActionResult> GetAllViolations()
        {
            try
            {
                var items = await _violationsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Violation>>.SuccessResponse(
                    items,
                    "Violations retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/violations/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetViolationsByEmployeeId([FromRoute] string employeeId)
        {
            try
            {
                var items = await _violationsRepo.GetByEmployeeIdAsync(employeeId);
                var response = ApiResponse<IEnumerable<Violation>>.SuccessResponse(
                    items,
                    "Violations retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Violation>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/violations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetViolationById([FromRoute] string id)
        {
            try
            {
                var item = await _violationsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Violation>.ErrorResponse(
                        "Violation not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Violation>.SuccessResponse(
                    item,
                    "Violation retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Violation>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/violations
        [HttpPost]
        public async Task<IActionResult> AddViolation([FromBody] Violation violation)
        {
            try
            {
                var newId = Guid.NewGuid().ToString();
                await _violationsRepo.AddAsync(newId, violation);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Violation added successfully"
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

        // PUT /api/v1/violations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateViolation([FromRoute] string id, [FromBody] Violation violation)
        {
            try
            {
                var rowsAffected = await _violationsRepo.UpdateAsync(id, violation);
                if (rowsAffected == 0)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Violation not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Violation updated successfully"
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

        // DELETE /api/v1/violations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViolation([FromRoute] string id)
        {
            try
            {
                var rowsAffected = await _violationsRepo.DeleteAsync(id);
                if (rowsAffected == 0)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Violation not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Violation deleted successfully"
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
