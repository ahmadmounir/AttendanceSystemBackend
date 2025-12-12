namespace AttendanceSystemBackend.Repositories.LeaveRequests
{
    public interface ILeaveRequestsRepo
    {
        Task<IEnumerable<Models.LeaveRequest>> GetAllAsync();

        Task<Models.LeaveRequest?> GetByIdAsync(string id);

        Task<string> AddAsync(Models.LeaveRequest leaveRequest);

        Task<Models.LeaveRequest> UpdateAsync(string id, Models.LeaveRequest leaveRequest);

        Task<Models.LeaveRequest> UpdateStatusAsync(string id, string status);

        Task<int> DeleteAsync(string id);
    }
}
