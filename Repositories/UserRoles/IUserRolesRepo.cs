namespace AttendanceSystemBackend.Repositories.UserRoles
{
    public interface IUserRolesRepo
    {
        Task<IEnumerable<Models.UserRole>> GetAllAsync();
        Task<Models.UserRole?> GetByIdAsync(string id);
    }
}
