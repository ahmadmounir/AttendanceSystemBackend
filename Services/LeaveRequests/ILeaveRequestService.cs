using AttendanceSystemBackend.Models;
using AttendanceSystemBackend.Models.DTOs;

namespace AttendanceSystemBackend.Services.LeaveRequests
{
    public interface ILeaveRequestService
    {
        Task<string> CreateLeaveRequestAsync(string employeeId, LeaveRequestCreateDto dto);
        Task<bool> ReviewLeaveRequestAsync(string requestId,  LeaveRequestReviewDto dto);
    }
}
