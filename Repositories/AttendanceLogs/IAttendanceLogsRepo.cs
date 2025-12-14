namespace AttendanceSystemBackend.Repositories.AttendanceLogs
{
    public interface IAttendanceLogsRepo
    {
        Task<IEnumerable<Models.AttendanceLog>> GetAllAsync();
        Task<IEnumerable<Models.DTOs.AttendanceLogWithEmployeeDto>> GetAllWithEmployeeAsync();


        Task<Models.AttendanceLog?> GetByIdAsync(string id);
        Task<Models.DTOs.AttendanceLogWithEmployeeDto?> GetByIdWithEmployeeAsync(string id);

        Task<IEnumerable<Models.AttendanceLog>> GetByEmployeeIdAsync(string employeeId);
        Task<IEnumerable<Models.DTOs.AttendanceLogWithEmployeeDto>> GetByEmployeeIdWithEmployeeAsync(string employeeId);

        Task<string> AddAsync(string empId);

        Task<string> UpdateAsync(string id);
    }
}
