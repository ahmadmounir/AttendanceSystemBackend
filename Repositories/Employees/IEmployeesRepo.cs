
namespace AttendanceSystemBackend.Repositories.Employees
{
    public interface IEmployeesRepo
    {
        Task<IEnumerable<Models.Employees>> GetAllAsync();

        Task<string> AddAsync(Models.Employees employee);

        Task<Models.Employees> UpdateAsync(string id, Models.Employees employee);

        Task<int> DeleteAsync(string id);
    }
}