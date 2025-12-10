namespace AttendanceSystemBackend.Repositories.Regions
{
    public interface IRegionsRepo
    {
        Task<IEnumerable<Models.Region>> GetAllAsync();
        Task<Models.Region?> GetByIdAsync(string id);
    }
}
