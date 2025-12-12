using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("notifications")]
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationsRepo _notificationsRepo;

        public NotificationsController(INotificationsRepo notificationsRepo)
        {
            _notificationsRepo = notificationsRepo;
        }

        // GET /api/v1/notifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<IEnumerable<Notification>>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var notifications = await _notificationsRepo.GetUserNotificationsAsync(userId);
                var response = ApiResponse<IEnumerable<Notification>>.SuccessResponse(
                    notifications,
                    "Notifications retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Notification>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // GET /api/v1/notifications/unread
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = ApiResponse<IEnumerable<Notification>>.ErrorResponse(
                        "Not authorized",
                        401
                    );
                    return Unauthorized(errorResponse);
                }

                var notifications = await _notificationsRepo.GetUnreadNotificationsAsync(userId);
                var response = ApiResponse<IEnumerable<Notification>>.SuccessResponse(
                    notifications,
                    "Unread notifications retrieved successfully"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<Notification>>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // PUT /api/v1/notifications/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] string id)
        {
            try
            {
                var result = await _notificationsRepo.MarkAsReadAsync(id);
                if (!result)
                {
                    var notFoundResponse = ApiResponse<bool>.ErrorResponse(
                        "Notification not found",
                        404
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<bool>.SuccessResponse(
                    true,
                    "Notification marked as read"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }

        // PUT /api/v1/notifications/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
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

                var result = await _notificationsRepo.MarkAllAsReadAsync(userId);
                var response = ApiResponse<bool>.SuccessResponse(
                    true,
                    "All notifications marked as read"
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    ex.Message,
                    500
                );
                return StatusCode(500, response);
            }
        }
    }
}
