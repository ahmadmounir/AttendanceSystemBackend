using AttendanceSystemBackend.Models.DTOs;

namespace AttendanceSystemBackend.Repositories.Departments
{
    public interface IDepartmentsRepo
    {
        Task<IEnumerable<Models.Department>> GetAllAsync();
        Task<IEnumerable<DepartmentWithEmployeeCountDto>> GetAllWithEmployeeCountAsync();
        Task<Models.Department?> GetByIdAsync(string id);
        Task<string> AddAsync(string name);
        Task<int> UpdateAsync(string id, string name);
        Task<int> DeleteAsync(string id);
        Task<int> GetCountAsync();
    }
}
