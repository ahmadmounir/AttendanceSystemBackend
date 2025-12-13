using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Shifts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("shifts")]
    [AllowAnonymous]
    public class ShiftsController : Controller
    {
        private readonly IShiftsRepo _shiftsRepo;

        public ShiftsController(IShiftsRepo shiftsRepo)
        {
            _shiftsRepo = shiftsRepo;
        }

        // GET /api/v1/shifts
        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            try
            {
                var items = await _shiftsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Shift>>.SuccessResponse(
                    items,
                    "Shifts retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Shift>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/shifts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShiftById([FromRoute] string id)
        {
            try
            {
                var item = await _shiftsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Shift>.ErrorResponse(
                        "Shift not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Shift>.SuccessResponse(
                    item,
                    "Shift retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Shift>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
