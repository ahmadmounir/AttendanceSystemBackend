using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.LeaveBalances;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
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

        // POST /api/v1/leavebalances
        [HttpPost]
        public async Task<IActionResult> AddLeaveBalance([FromBody] Models.LeaveBalance leaveBalance)
        {
            try
            {
                var newId = await _leaveBalancesRepo.AddAsync(leaveBalance);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Leave balance added successfully"
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

        // PUT /api/v1/leavebalances/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveBalance([FromRoute] string id, [FromBody] Models.LeaveBalance leaveBalance)
        {
            try
            {
                var updated = await _leaveBalancesRepo.UpdateAsync(id, leaveBalance);
                var response = ApiResponse<Models.LeaveBalance>.SuccessResponse(
                    updated,
                    "Leave balance updated successfully"
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
