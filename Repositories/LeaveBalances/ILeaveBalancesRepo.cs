namespace AttendanceSystemBackend.Repositories.LeaveBalances
{
    public interface ILeaveBalancesRepo
    {
        Task<IEnumerable<Models.LeaveBalance>> GetAllAsync();
        Task<IEnumerable<Models.LeaveBalance>> GetEmployeeBalancesAsync(string employeeId);
        Task<Models.LeaveBalance?> GetByIdAsync(string id);
        Task<Models.LeaveBalance?> GetByEmployeeAndTypeAsync(string employeeId, string leaveTypeId);
        Task<bool> DeductLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days);
        Task<string> AddAsync(Models.LeaveBalance leaveBalance);
        Task<bool> AdjustLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days);
    }
}
