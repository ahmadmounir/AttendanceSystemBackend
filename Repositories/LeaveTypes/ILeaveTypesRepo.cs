namespace AttendanceSystemBackend.Repositories.LeaveTypes
{
    public interface ILeaveTypesRepo
    {
        Task<IEnumerable<Models.LeaveType>> GetAllAsync();
        Task<Models.LeaveType?> GetByIdAsync(string id);
    }
}
