namespace AttendanceSystemBackend.Repositories.Violations
{
    public interface IViolationsRepo
    {
        Task<IEnumerable<Models.Violation>> GetAllAsync();
        Task<Models.Violation?> GetByIdAsync(string id);
        Task<IEnumerable<Models.Violation>> GetByEmployeeIdAsync(string employeeId);
        Task<string> AddAsync(Models.Violation violation);
        Task<int> UpdateAsync(string id, Models.Violation violation);
        Task<int> DeleteAsync(string id);
    }
}
