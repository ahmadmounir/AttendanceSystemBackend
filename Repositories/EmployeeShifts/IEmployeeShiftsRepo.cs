namespace AttendanceSystemBackend.Repositories.EmployeeShifts
{
    public interface IEmployeeShiftsRepo
    {
        Task<IEnumerable<Models.EmployeeShift>> GetAllAsync();
        Task<Models.EmployeeShift?> GetByEmployeeAndStartDateAsync(string employeeId, DateOnly startDate);
        Task<IEnumerable<Models.EmployeeShift>> GetByEmployeeIdAsync(string employeeId);
        Task AddAsync(Models.EmployeeShift employeeShift);
        Task<int> UpdateAsync(string employeeId, DateOnly startDate, Models.EmployeeShift employeeShift);
        Task<int> DeleteAsync(string employeeId, DateOnly startDate);
    }
}
