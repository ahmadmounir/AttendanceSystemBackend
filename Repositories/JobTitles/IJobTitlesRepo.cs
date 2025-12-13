namespace AttendanceSystemBackend.Repositories.JobTitles
{
    public interface IJobTitlesRepo
    {
        Task<IEnumerable<Models.JobTitle>> GetAllAsync();
        Task<Models.JobTitle?> GetByIdAsync(string id);
        Task<string> AddAsync(string titleName, float minSalary, float maxSalary);
        Task<Models.JobTitle> UpdateAsync(string id, string titleName, float minSalary, float maxSalary);
        Task<int> DeleteAsync(string id);
    }
}
