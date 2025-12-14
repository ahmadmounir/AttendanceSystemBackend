namespace AttendanceSystemBackend.Repositories.LeaveRequests
{
    public interface ILeaveRequestsRepo
    {
        Task<IEnumerable<Models.LeaveRequest>> GetAllAsync();
        Task<IEnumerable<Models.LeaveRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<Models.LeaveRequest>> GetEmployeeRequestsAsync(string employeeId);
        Task<IEnumerable<Models.DTOs.LeaveRequestWithBalanceDto>> GetEmployeeRequestsWithBalanceAsync(string employeeId);
        Task<Models.LeaveRequest?> GetByIdAsync(string id);
        Task<string> AddAsync(Models.LeaveRequest leaveRequest);
        Task<Models.LeaveRequest> UpdateAsync(string id, Models.LeaveRequest leaveRequest);
        Task<bool> ReviewRequestAsync(string id, string status, string reviewedBy, string? reviewNotes);
        Task<int> DeleteAsync(string id);
        Task<int> GetPendingCountAsync();
    }
}
