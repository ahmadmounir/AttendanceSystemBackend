namespace AttendanceSystemBackend.Repositories.LeaveBalances
{
    public interface ILeaveBalancesRepo
    {
        Task<IEnumerable<Models.LeaveBalance>> GetAllAsync();
        Task<IEnumerable<Models.LeaveBalance>> GetEmployeeBalancesAsync(string employeeId);
        Task<Models.LeaveBalance?> GetByIdAsync(string id);
        Task<Models.LeaveBalance?> GetByEmployeeAndTypeAsync(string employeeId, string leaveTypeId, int year);
        Task<bool> DeductLeaveBalanceAsync(string employeeId, string leaveTypeId, int year, decimal days);
    }
}
