using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using AttendanceSystemBackend.Services.LeaveRequests;
using AttendanceSystemBackend.Repositories.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("leaverequests")]
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly ILeaveRequestsRepo _leaveRequestsRepo;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly INotificationsRepo _notificationsRepo;

        public LeaveRequestsController(
            ILeaveRequestsRepo leaveRequestsRepo,
            ILeaveRequestService leaveRequestService,
            INotificationsRepo notificationsRepo)
        {
            _leaveRequestsRepo = leaveRequestsRepo;
            _leaveRequestService = leaveRequestService;
            _notificationsRepo = notificationsRepo;
        }

        // GET /api/v1/leaverequests (Admin only - all requests)
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            try
            {
                var items = await _leaveRequestsRepo.GetAllWithDetailsAsync();
                var response = ApiResponse<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>>.SuccessResponse(
                    items,
                    "Leave requests retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leaverequests/pending (Admin only - pending requests)
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var items = await _leaveRequestsRepo.GetPendingWithDetailsAsync();
                var response = ApiResponse<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>>.SuccessResponse(
                    items,
                    "Pending leave requests retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/leaverequests/me/{id} (Employee - own requests)
        [HttpGet("me/{id}")]
        public async Task<IActionResult> GetMyLeaveRequests([FromRoute] string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<IEnumerable<LeaveRequest>>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var items = await _leaveRequestsRepo.GetEmployeeRequestsAsync(id);
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

        // POST /api/v1/leaverequests/{id} (Employee creates request)
        [HttpPost("{id}")]
        public async Task<IActionResult> CreateLeaveRequest([FromRoute] string id, [FromBody] LeaveRequestCreateDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<string>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var newId = await _leaveRequestService.CreateLeaveRequestAsync(id, dto);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Leave request submitted successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message,
                    400
                );
                return BadRequest(response);
            }
        }

        // PUT /api/v1/leaverequests/{id}/review (Admin approves/rejects)
        [HttpPut("{id}/review")]
        public async Task<IActionResult> ReviewLeaveRequest([FromRoute] string id, [FromBody] LeaveRequestReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<bool>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var request = await _leaveRequestsRepo.GetByIdAsync(id);
                if (request == null)
                {
                    var notFoundResponse = ApiResponse<bool>.ErrorResponse(
                        "Leave request not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var success = await _leaveRequestService.ReviewLeaveRequestAsync(id, dto);
                var message = dto.Status == "Approved" 
                    ? "Leave request approved successfully" 
                    : "Leave request rejected successfully";

                // Create notification for employee
                var notification = new Models.Notification
                {
                    Title = message,
                    Description = dto.Status == "Approved"
                        ? $"Your leave request from {request.StartDate:MMM dd, yyyy} to {request.EndDate:MMM dd, yyyy} has been approved."
                        : $"Your leave request from {request.StartDate:MMM dd, yyyy} to {request.EndDate:MMM dd, yyyy} has been rejected.",
                    EmployeeId = request.EmployeeId,
                    MarkedAsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _notificationsRepo.AddAsync(notification);

                var response = ApiResponse<bool>.SuccessResponse(
                    success,
                    message
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    ex.Message,
                    400
                );
                return BadRequest(response);
            }
        }

        // DELETE /api/v1/leaverequests/{id} (Only pending requests can be deleted by employee)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest([FromRoute] string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<int>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var request = await _leaveRequestsRepo.GetByIdAsync(id);
                if (request == null)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Leave request not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                if (request.Status != "Pending")
                {
                    var errorResponse = ApiResponse<int>.ErrorResponse(
                        "Only pending requests can be canceled",
                        400
                    );
                    return BadRequest(errorResponse);
                }

                var rowsAffected = await _leaveRequestsRepo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rowsAffected,
                    "Leave request canceled successfully"
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
