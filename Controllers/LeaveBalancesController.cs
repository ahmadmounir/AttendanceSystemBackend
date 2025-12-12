using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.LeaveBalances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystemBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("leavebalances")]
    public class LeaveBalancesController : Controller
    {
        private readonly ILeaveBalancesRepo _leaveBalancesRepo;

        public LeaveBalancesController(ILeaveBalancesRepo leaveBalancesRepo)
        {
            _leaveBalancesRepo = leaveBalancesRepo;
        }

        // GET /api/v1/leavebalances
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveBalances()
        {
            try
            {
                var items = await _leaveBalancesRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.LeaveBalance>>.SuccessResponse(
                    items,
                    "Leave balances retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.LeaveBalance>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leavebalances/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLeaveBalances()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<IEnumerable<LeaveBalance>>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var items = await _leaveBalancesRepo.GetEmployeeBalancesAsync(userId);
                var response = ApiResponse<IEnumerable<Models.LeaveBalance>>.SuccessResponse(
                    items,
                    "Leave balances retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.LeaveBalance>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leavebalances/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveBalanceById([FromRoute] string id)
        {
            try
            {
                var item = await _leaveBalancesRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Models.LeaveBalance>.ErrorResponse(
                        "Leave balance not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Models.LeaveBalance>.SuccessResponse(
                    item,
                    "Leave balance retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.LeaveBalance>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
