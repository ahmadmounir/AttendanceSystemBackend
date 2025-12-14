namespace AttendanceSystemBackend.Repositories.LeaveBalances
{
    public interface ILeaveBalancesRepo
    {
        Task<IEnumerable<Models.LeaveBalance>> GetAllAsync();
        Task<IEnumerable<Models.LeaveBalance>> GetEmployeeBalancesAsync(string employeeId);
        Task<Models.LeaveBalance?> GetByIdAsync(string id);
        Task<Models.LeaveBalance?> GetByEmployeeAndTypeAsync(string employeeId, string leaveTypeId);
        Task<bool> DeductLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days);
        Task AddAsync(Models.LeaveBalance balance);
        Task AdjustLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days);
    }
}
