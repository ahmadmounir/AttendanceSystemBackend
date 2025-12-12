using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using AttendanceSystemBackend.Repositories.LeaveBalances;
using AttendanceSystemBackend.Repositories.Notifications;
using AttendanceSystemBackend.Repositories.Auth;

namespace AttendanceSystemBackend.Services.LeaveRequests
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestsRepo _leaveRequestsRepo;
        private readonly ILeaveBalancesRepo _leaveBalancesRepo;
        private readonly INotificationsRepo _notificationsRepo;
        private readonly IAuthRepo _authRepo;

        public LeaveRequestService(
            ILeaveRequestsRepo leaveRequestsRepo,
            ILeaveBalancesRepo leaveBalancesRepo,
            INotificationsRepo notificationsRepo,
            IAuthRepo authRepo)
        {
            _leaveRequestsRepo = leaveRequestsRepo;
            _leaveBalancesRepo = leaveBalancesRepo;
            _notificationsRepo = notificationsRepo;
            _authRepo = authRepo;
        }

        public async Task<string> CreateLeaveRequestAsync(string employeeId, LeaveRequestCreateDto dto)
        {
            var days = (decimal)(dto.EndDate - dto.StartDate).TotalDays + 1;

            var currentYear = DateTime.UtcNow.Year;
            var balance = await _leaveBalancesRepo.GetByEmployeeAndTypeAsync(
                employeeId, 
                dto.LeaveTypeId, 
                currentYear
            );

            if (balance == null)
            {
                throw new Exception("No leave balance found for this leave type");
            }

            if (balance.RemainingDays < days)
            {
                throw new Exception($"Insufficient leave balance. Available: {balance.RemainingDays} days, Requested: {days} days");
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = "Pending"
            };

            return await _leaveRequestsRepo.AddAsync(leaveRequest);
        }

        public async Task<bool> ReviewLeaveRequestAsync(string requestId, string reviewedBy, LeaveRequestReviewDto dto)
        {
            if (dto.Status != "Approved" && dto.Status != "Rejected")
            {
                throw new ArgumentException("Status must be either 'Approved' or 'Rejected'");
            }

            var request = await _leaveRequestsRepo.GetByIdAsync(requestId);
            if (request == null)
            {
                throw new Exception("Leave request not found");
            }

            if (request.Status != "Pending")
            {
                throw new Exception("Only pending requests can be reviewed");
            }

            var success = await _leaveRequestsRepo.ReviewRequestAsync(
                requestId, 
                dto.Status, 
                reviewedBy, 
                dto.ReviewNotes
            );

            if (!success) return false;

            if (dto.Status == "Approved")
            {
                var days = (decimal)(request.EndDate - request.StartDate).TotalDays + 1;
                var currentYear = DateTime.UtcNow.Year;
                var deducted = await _leaveBalancesRepo.DeductLeaveBalanceAsync(
                    request.EmployeeId,
                    request.LeaveTypeId,
                    currentYear,
                    days
                );

                if (!deducted)
                {
                    throw new Exception("Failed to deduct leave balance. Insufficient balance or balance not found.");
                }
            }

            var user = await _authRepo.GetUserByIdAsync(reviewedBy);
            var reviewerName = user?.Username ?? "Admin";
            
            var notificationMessage = dto.Status == "Approved"
                ? $"Your leave request from {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd} has been approved by {reviewerName}"
                : $"Your leave request from {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd} has been rejected by {reviewerName}";

            if (!string.IsNullOrEmpty(dto.ReviewNotes))
            {
                notificationMessage += $". Note: {dto.ReviewNotes}";
            }

            var userAccount = await _authRepo.GetUserByIdAsync(request.EmployeeId);
            if (userAccount != null)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userAccount.Id,
                    Message = notificationMessage,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationsRepo.CreateNotificationAsync(notification);
            }

            return true;
        }
    }
}
