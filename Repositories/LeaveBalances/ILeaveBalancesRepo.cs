namespace AttendanceSystemBackend.Repositories.LeaveBalances
{
    public interface ILeaveBalancesRepo
    {
        Task<IEnumerable<Models.LeaveBalance>> GetAllAsync();

        Task<Models.LeaveBalance?> GetByIdAsync(string id);

        Task<string> AddAsync(Models.LeaveBalance leaveBalance);

        Task<Models.LeaveBalance> UpdateAsync(string id, Models.LeaveBalance leaveBalance);
    }
}
