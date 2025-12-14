using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;
using AttendanceSystemBackend.Repositories.LeaveRequests;
using AttendanceSystemBackend.Repositories.LeaveBalances;
using AttendanceSystemBackend.Repositories.Auth;

namespace AttendanceSystemBackend.Services.LeaveRequests
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestsRepo _leaveRequestsRepo;
        private readonly ILeaveBalancesRepo _leaveBalancesRepo;
        private readonly IAuthRepo _authRepo;

        public LeaveRequestService(
            ILeaveRequestsRepo leaveRequestsRepo,
            ILeaveBalancesRepo leaveBalancesRepo,
            IAuthRepo authRepo)
        {
            _leaveRequestsRepo = leaveRequestsRepo;
            _leaveBalancesRepo = leaveBalancesRepo;
            _authRepo = authRepo;
        }

        public async Task<string> CreateLeaveRequestAsync(string employeeId, LeaveRequestCreateDto dto)
        {
            var days = (decimal)(dto.EndDate - dto.StartDate).TotalDays + 1;

            var balance = await _leaveBalancesRepo.GetByEmployeeAndTypeAsync(
                employeeId, 
                dto.LeaveTypeId
            );

            if (balance == null)
            {
                balance = new Models.LeaveBalance
                {
                    Id = Guid.NewGuid().ToString(),
                    EmployeeId = employeeId,
                    LeaveTypeId = dto.LeaveTypeId,
                    RemainingDays = 20
                };

                await _leaveBalancesRepo.AddAsync(balance);
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

        public async Task<bool> ReviewLeaveRequestAsync(string requestId, LeaveRequestReviewDto dto)
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

            var success = await _leaveRequestsRepo.ReviewRequestAsync(requestId, dto.Status);

            if (!success) return false;

            if (dto.Status == "Approved")
            {
                var days = (decimal)(request.EndDate - request.StartDate).TotalDays;

                var balance = await _leaveBalancesRepo.GetByEmployeeAndTypeAsync(request.EmployeeId, request.LeaveTypeId);
                if (balance == null)
                {
                    throw new Exception("Leave balance not found for the employee and leave type.");
                }
                else
                {
                    var remainingDays = balance.RemainingDays - days; // Assume initial balance is 20 days
                    if (remainingDays < 0)
                    {
                        throw new Exception("Your remining days is lower then the Balance.");
                    }
                    var newBalance = new Models.LeaveBalance
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = request.EmployeeId,
                        LeaveTypeId = request.LeaveTypeId,
                        RemainingDays = remainingDays
                    };
                    // Adjust existing balance by subtracting days (allow negative)
                    await _leaveBalancesRepo.AdjustLeaveBalanceAsync(request.EmployeeId, request.LeaveTypeId, days);
                }
            }
            return true;
        }
    }
}
