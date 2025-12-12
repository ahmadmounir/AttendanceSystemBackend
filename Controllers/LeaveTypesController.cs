using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("leavetypes")]
    [Authorize]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypesRepo _leaveTypesRepo;

        public LeaveTypesController(ILeaveTypesRepo leaveTypesRepo)
        {
            _leaveTypesRepo = leaveTypesRepo;
        }

        // GET /api/v1/leavetypes
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            try
            {
                var items = await _leaveTypesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<LeaveType>>.SuccessResponse(
                    items,
                    "Leave types retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<LeaveType>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leavetypes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveTypeById([FromRoute] string id)
        {
            try
            {
                var item = await _leaveTypesRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<LeaveType>.ErrorResponse(
                        "Leave type not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<LeaveType>.SuccessResponse(
                    item,
                    "Leave type retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<LeaveType>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
