namespace AttendanceSystemBackend.Repositories.Shifts
{
    public interface IShiftsRepo
    {
        Task<IEnumerable<Models.Shift>> GetAllAsync();
        Task<Models.Shift?> GetByIdAsync(string id);
    }
}
