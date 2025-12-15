using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.OvertimeRequests;
using AttendanceSystemBackend.Services.Authorization;
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
        private readonly IUserAuthorizationService _authorizationService;

        public OvertimeRequestsController(IOvertimeRequestsRepo repo, IUserAuthorizationService authorizationService)
        {
            _repo = repo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/overtimerequests (Admin sees all)
        [HttpGet]
        public async Task<IActionResult> GetAllOvertimeRequests()
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateAdminAccessAsync(User);
                if (!isAuthorized)
                {
                    var statusCode = errorMessage == "Not authorized" ? 401 : 403;
                    return StatusCode(statusCode, ApiResponse<IEnumerable<Models.DTOs.OvertimeRequestWithEmployeeDto>>.ErrorResponse(
                        errorMessage ?? "Access denied", statusCode));
                }

                var items = await _repo.GetAllWithEmployeeAsync();
                var response = ApiResponse<IEnumerable<Models.DTOs.OvertimeRequestWithEmployeeDto>>.SuccessResponse(
                    items,
                    "Overtime requests retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Models.DTOs.OvertimeRequestWithEmployeeDto>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/overtimerequests/my/{id} (Employee sees own requests)
        [HttpGet("my/{id}")]
        public async Task<IActionResult> GetMyOvertimeRequests([FromRoute] string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<IEnumerable<Models.OvertimeRequest>>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var items = await _repo.GetByEmployeeIdAsync(id);
                var response = ApiResponse<IEnumerable<Models.OvertimeRequest>>.SuccessResponse(
                    items,
                    "Your overtime requests retrieved successfully"
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

        // POST /api/v1/overtimerequests/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> AddOvertimeRequest([FromRoute] string id, [FromBody] OvertimeRequestCreateDto dto)
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
                    EmployeeId = id,
                    RequestDate = dto.RequestDate,
                    Hours = dto.Hours,
                    Reason = dto.Reason,
                    Status = "Pending"
                };
                // Ensure pending by default
                if (string.IsNullOrWhiteSpace(request.Status))
                    request.Status = "Pending";

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
        public async Task<IActionResult> UpdateRequestStatus([FromRoute] string id, [FromBody] Models.DTOs.OvertimeRequestStatusDto request)
        {
            try
            {
                var rows = await _repo.UpdateApprovalStatusAsync(id, request.Status);
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


                var rows = await _repo.DeleteAsync(id);
                var response = ApiResponse<int>.SuccessResponse(
                    rows,
                    "Overtime request canceled successfully"
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
