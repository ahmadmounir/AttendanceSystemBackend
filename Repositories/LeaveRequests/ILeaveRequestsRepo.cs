namespace AttendanceSystemBackend.Repositories.LeaveRequests
{
    public interface ILeaveRequestsRepo
    {
        Task<IEnumerable<Models.LeaveRequest>> GetAllAsync();
        Task<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>> GetAllWithDetailsAsync();
        Task<IEnumerable<Models.LeaveRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>> GetPendingWithDetailsAsync();
        Task<IEnumerable<Models.LeaveRequest>> GetEmployeeRequestsAsync(string employeeId);
        Task<Models.LeaveRequest?> GetByIdAsync(string id);
        Task<string> AddAsync(Models.LeaveRequest leaveRequest);
        Task<Models.LeaveRequest> UpdateAsync(string id, Models.LeaveRequest leaveRequest);
        Task<bool> ReviewRequestAsync(string id, string status);
        Task<int> DeleteAsync(string id);
        Task<int> GetPendingCountAsync();
    }
}
