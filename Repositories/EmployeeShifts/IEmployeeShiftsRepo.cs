namespace AttendanceSystemBackend.Repositories.EmployeeShifts
{
    public interface IEmployeeShiftsRepo
    {
        Task<IEnumerable<Models.EmployeeShift>> GetAllAsync();
        Task<Models.EmployeeShift?> GetByEmployeeAndStartDateAsync(string employeeId, DateTime startDate);
        Task<IEnumerable<Models.EmployeeShift>> GetByEmployeeIdAsync(string employeeId);
        Task AddAsync(Models.EmployeeShift employeeShift);
        Task<int> UpdateAsync(string employeeId, DateTime startDate, Models.EmployeeShift employeeShift);
        Task<int> DeleteAsync(string employeeId, DateTime startDate);
    }
}
