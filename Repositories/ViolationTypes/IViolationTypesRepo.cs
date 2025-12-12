namespace AttendanceSystemBackend.Repositories.ViolationTypes
{
    public interface IViolationTypesRepo
    {
        Task<IEnumerable<Models.ViolationType>> GetAllAsync();
        Task<Models.ViolationType?> GetByIdAsync(string id);
    }
}
