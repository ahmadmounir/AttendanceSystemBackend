using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.ViolationTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("violationtypes")]
    [Authorize]
    public class ViolationTypesController : Controller
    {
        private readonly IViolationTypesRepo _violationTypesRepo;

        public ViolationTypesController(IViolationTypesRepo violationTypesRepo)
        {
            _violationTypesRepo = violationTypesRepo;
        }

        // GET /api/v1/violationtypes
        [HttpGet]
        public async Task<IActionResult> GetAllViolationTypes()
        {
            try
            {
                var items = await _violationTypesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<ViolationType>>.SuccessResponse(
                    items,
                    "Violation types retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ViolationType>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/violationtypes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetViolationTypeById([FromRoute] string id)
        {
            try
            {
                var item = await _violationTypesRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<ViolationType>.ErrorResponse(
                        "Violation type not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<ViolationType>.SuccessResponse(
                    item,
                    "Violation type retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ViolationType>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
