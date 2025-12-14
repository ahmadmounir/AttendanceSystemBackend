using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.OvertimeRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("overtimerequests")]
    [Authorize]
    public class OvertimeRequestsController : Controller
    {
        private readonly IOvertimeRequestsRepo _repo;

        public OvertimeRequestsController(IOvertimeRequestsRepo repo)
        {
            _repo = repo;
        }

        // GET /api/v1/overtimerequests
        [HttpGet]
        public async Task<IActionResult> GetAllOvertimeRequests()
        {
            try
            {
                var items = await _repo.GetAllAsync();
                var response = ApiResponse<IEnumerable<Models.OvertimeRequest>>.SuccessResponse(
                    items,
                    "Overtime requests retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.OvertimeRequest>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/overtimerequests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOvertimeRequestById([FromRoute] string id)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id);
                if (item == null)
                {
                    var notFoundResponse = ApiResponse<Models.OvertimeRequest>.ErrorResponse(
                        "Overtime request not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<Models.OvertimeRequest>.SuccessResponse(
                    item,
                    "Overtime request retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<Models.OvertimeRequest>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // POST /api/v1/overtimerequests
        [HttpPost]
        public async Task<IActionResult> AddOvertimeRequest([FromBody] OvertimeRequestCreateDto dto)
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

                // Create overtime request with employee and status
                var request = new Models.OvertimeRequest
                {
                    EmployeeId = userId,
                    RequestDate = dto.RequestDate,
                    Hours = dto.Hours,
                    Reason = dto.Reason,
                    Status = dto.Status ?? "Pending"
                };

                var newId = await _repo.AddAsync(request);
                var response = ApiResponse<string>.SuccessResponse(
                    newId,
                    "Overtime request submitted successfully"
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

        // PUT /api/v1/overtimerequests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOvertimeRequest([FromRoute] string id, [FromBody] Models.OvertimeRequest request)
        {
            try
            {
                request.Id = id;
                var rows = await _repo.UpdateAsync(request);
                var response = ApiResponse<int>.SuccessResponse(
                    rows,
                    "Overtime request updated successfully"
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

        // PUT /api/v1/overtimerequests/{id}/approval
        [HttpPut("{id}/approval")]
        public async Task<IActionResult> UpdateRequestStatus([FromRoute] string id, [FromBody] dynamic data)
        {
            try
            {
                string status = data.status;
                var rows = await _repo.UpdateApprovalStatusAsync(id, status);
                var updated = await _repo.GetByIdAsync(id);
                var response = ApiResponse<Models.OvertimeRequest>.SuccessResponse(
                    updated,
                    "Overtime request approval status updated"
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

        // DELETE /api/v1/overtimerequests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOvertimeRequest([FromRoute] string id)
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

                var request = await _repo.GetByIdAsync(id);
                if (request == null)
                {
                    var notFoundResponse = ApiResponse<int>.ErrorResponse(
                        "Overtime request not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                if (request.EmployeeId != userId)
                {
                    var forbiddenResponse = ApiResponse<int>.ErrorResponse(
                        "You can only delete your own requests",
                        403
                    );
                    return StatusCode(403, forbiddenResponse);
                }

                var rows = await _repo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rows,
                    "Overtime request deleted successfully"
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
