using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Repositories.Notifications;
using AttendanceSystemBackend.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystemBackend.Controllers
{
    [ApiController]
    [Route("notifications")]
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationsRepo _notificationsRepo;
        private readonly IUserAuthorizationService _authorizationService;

        public NotificationsController(INotificationsRepo notificationsRepo, IUserAuthorizationService authorizationService)
        {
            _notificationsRepo = notificationsRepo;
            _authorizationService = authorizationService;
        }

        // GET /api/v1/notifications (Employee sees own notifications)
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            try
            {
                var employeeId = await _authorizationService.GetCurrentEmployeeIdAsync(User);
                if (string.IsNullOrEmpty(employeeId))
                {
                    return NotFound(ApiResponse<IEnumerable<Models.Notification>>.ErrorResponse(
                        "Employee not found", 404));
                }

                var notifications = await _notificationsRepo.GetByEmployeeIdAsync(employeeId);
                return Ok(ApiResponse<IEnumerable<Models.Notification>>.SuccessResponse(
                    notifications, "Your notifications retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<Models.Notification>>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // POST /api/v1/notifications/{employeeId} (Create notification for employee)
        [HttpPost("{employeeId}")]
        public async Task<IActionResult> CreateNotification([FromRoute] string employeeId, [FromBody] CreateNotificationRequest request)
        {
            try
            {
                var (isAuthorized, errorMessage) = await _authorizationService.ValidateEmployeeAccessAsync(User, employeeId);
                if (!isAuthorized)
                {
                    return StatusCode(403, ApiResponse<string>.ErrorResponse(
                        errorMessage ?? "Access denied", 403));
                }

                var notification = new Models.Notification
                {
                    Title = request.Title,
                    Descr = request.Description,
                    EmployeeId = employeeId,
                    MarkedAsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _notificationsRepo.AddAsync(notification);
                return Ok(ApiResponse<string>.SuccessResponse(
                    notification.Id, "Notification created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }

        // PUT /api/v1/notifications/{id} (Mark notification as read using token)
        [HttpPut("{id}")]
        public async Task<IActionResult> MarkNotificationAsRead([FromRoute] string id)
        {
            try
            {
                var employeeId = await _authorizationService.GetCurrentEmployeeIdAsync(User);
                if (string.IsNullOrEmpty(employeeId))
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(
                        "Employee not found", 404));
                }

                var notification = await _notificationsRepo.GetByIdAsync(id);
                if (notification == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(
                        "Notification not found", 404));
                }

                var result = await _notificationsRepo.MarkAsReadAsync(id);
                if (result > 0)
                {
                    return Ok(ApiResponse<string>.SuccessResponse(
                        employeeId, "Notification marked as read successfully"));
                }
                else
                {
                    return StatusCode(500, ApiResponse<string>.ErrorResponse(
                        "Failed to update notification", 500));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    ex.Message, 500));
            }
        }
    }

    public class CreateNotificationRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}
