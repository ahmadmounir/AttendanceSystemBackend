using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("leaverequests")]
    public class LeaveRequestsController : Controller
    {
        private readonly ILeaveRequestsRepo _leaveRequestsRepo;

        public LeaveRequestsController(ILeaveRequestsRepo leaveRequestsRepo)
        {
            _leaveRequestsRepo = leaveRequestsRepo;
        }

        // GET /api/v1/leaverequests
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            try
            {
                var items = await _leaveRequestsRepo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.LeaveRequest>>.SuccessResponse(
                    items,
                    "Leave requests retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.LeaveRequest>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leaverequests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById([FromRoute] string id)
        {
            try
            {
                var item = await _leaveRequestsRepo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Models.LeaveRequest>.ErrorResponse(
                        "Leave request not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Models.LeaveRequest>.SuccessResponse(
                    item,
                    "Leave request retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.LeaveRequest>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/leaverequests
        [HttpPost]
        public async Task<IActionResult> AddLeaveRequest([FromBody] Models.LeaveRequest leaveRequest)
        {
            try
            {
                var newId = await _leaveRequestsRepo.AddAsync(leaveRequest);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Leave request added successfully"
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

        // PUT /api/v1/leaverequests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest([FromRoute] string id, [FromBody] Models.LeaveRequest leaveRequest)
        {
            try
            {
                var updated = await _leaveRequestsRepo.UpdateAsync(id, leaveRequest);
                var response = ApiResponse<Models.LeaveRequest>.SuccessResponse(
                    updated,
                    "Leave request updated successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.LeaveRequest>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // PUT /api/v1/leaverequests/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateLeaveRequestStatus([FromRoute] string id, [FromBody] dynamic statusData)
        {
            try
            {
                string status = statusData.status;
                var updated = await _leaveRequestsRepo.UpdateStatusAsync(id, status);
                var response = ApiResponse<Models.LeaveRequest>.SuccessResponse(
                    updated,
                    "Leave request status updated successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.LeaveRequest>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // DELETE /api/v1/leaverequests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest([FromRoute] string id)
        {
            try
            {
                var rowsAffected = await _leaveRequestsRepo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Leave request deleted successfully"
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
