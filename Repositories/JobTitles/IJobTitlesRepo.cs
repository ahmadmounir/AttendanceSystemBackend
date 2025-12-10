namespace AttendanceSystemBackend.Repositories.JobTitles
{
    public interface IJobTitlesRepo
    {
        Task<IEnumerable<Models.JobTitle>> GetAllAsync();
        Task<Models.JobTitle?> GetByIdAsync(string id);

        Task<string> AddAsync(Models.JobTitle jobTitle);

        Task<Models.JobTitle> UpdateAsync(string id, Models.JobTitle jobTitle);

        Task<int> DeleteAsync(string id);
    }
}
