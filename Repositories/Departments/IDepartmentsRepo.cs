namespace AttendanceSystemBackend.Repositories.Departments
{
    public interface IDepartmentsRepo
    {
        Task<IEnumerable<Models.Department>> GetAllAsync();
        Task<Models.Department?> GetByIdAsync(string id);
  
        Task<string> AddAsync(string id,Models.Department department);
         Task<int> DeleteAsync(string id);
    }
}
