namespace AttendanceSystemBackend.Repositories.Countries
{
    public interface ICountriesRepo
    {
        Task<IEnumerable<Models.Country>> GetAllAsync();
        Task<Models.Country?> GetByIdAsync(string id);
    }
}
