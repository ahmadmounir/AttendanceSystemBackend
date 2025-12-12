namespace AttendanceSystemBackend.Repositories.AttendanceLogs
{
    public interface IAttendanceLogsRepo
    {
        Task<IEnumerable<Models.AttendanceLog>> GetAllAsync();

        Task<Models.AttendanceLog?> GetByIdAsync(string id);

        Task<IEnumerable<Models.AttendanceLog>> GetByEmployeeIdAsync(string employeeId);

        Task<string> AddAsync(Models.AttendanceLog attendanceLog);

        Task<Models.AttendanceLog> UpdateAsync(string id, Models.AttendanceLog attendanceLog);
    }
}
