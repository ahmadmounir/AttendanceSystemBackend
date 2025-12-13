namespace AttendanceSystemBackend.Repositories.OvertimeRequests
{
    public interface IOvertimeRequestsRepo
    {
        Task<IEnumerable<Models.OvertimeRequest>> GetAllAsync();

        Task<Models.OvertimeRequest?> GetByIdAsync(string id);

        Task<string> AddAsync(Models.OvertimeRequest request);

        Task<int> UpdateAsync(Models.OvertimeRequest request);

        Task<int> UpdateApprovalStatusAsync(string id, string status);

        Task<int> DeleteAsync(string id);

        Task<int> GetPendingCountAsync();
    }
}
