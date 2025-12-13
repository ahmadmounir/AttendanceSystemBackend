using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.AttendanceLogs
{
    public class AttendanceLogsRepo : IAttendanceLogsRepo
    {
        private readonly string _connectionString;

        public AttendanceLogsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.AttendanceLog>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM AttendanceLogs";
            return await connection.QueryAsync<Models.AttendanceLog>(sql);
        }

        public async Task<IEnumerable<Models.DTOs.AttendanceLogWithEmployeeDto>> GetAllWithEmployeeAsync()
        {
            using var connection = CreateConnection();

            // Priority:
            // 1) Today's logs first
            // 2) Within that, employees who haven't clocked out (ClockOutTime IS NULL)
            // 3) Order by clockInTime desc
            // 4) Limit to top 100
            var sql = @"
SELECT TOP (100)
    a.id,
    a.employeeId,
    (e.firstName + ' ' + e.lastName) AS employeeFullName,
    a.clockInTime,
    a.clockOutTime,
    a.totalHours,
    a.doorLocation,
    a.exceptionType
FROM AttendanceLogs a
LEFT JOIN Employees e ON e.id = a.employeeId
ORDER BY
    CASE WHEN CONVERT(date, a.clockInTime) = CONVERT(date, GETDATE()) THEN 0 ELSE 1 END,
    CASE WHEN a.clockOutTime IS NULL THEN 0 ELSE 1 END,
    a.clockInTime DESC
";

            return await connection.QueryAsync<Models.DTOs.AttendanceLogWithEmployeeDto>(sql);
        }

        public async Task<Models.AttendanceLog?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM AttendanceLogs WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.AttendanceLog>(sql, parameters);
        }

        public async Task<Models.DTOs.AttendanceLogWithEmployeeDto?> GetByIdWithEmployeeAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT a.id, a.employeeId, (e.firstName + ' ' + e.lastName) AS employeeFullName,
                a.clockInTime, a.clockOutTime, a.totalHours, a.doorLocation, a.exceptionType
                FROM AttendanceLogs a
                LEFT JOIN Employees e ON e.id = a.employeeId
                WHERE a.id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Models.DTOs.AttendanceLogWithEmployeeDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Models.AttendanceLog>> GetByEmployeeIdAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM AttendanceLogs WHERE employeeId = @EmployeeId ORDER BY clockInTime DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.AttendanceLog>(sql, parameters);
        }

        public async Task<IEnumerable<Models.DTOs.AttendanceLogWithEmployeeDto>> GetByEmployeeIdWithEmployeeAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT TOP (100) a.id, a.employeeId, (e.firstName + ' ' + e.lastName) AS employeeFullName,
                a.clockInTime, a.clockOutTime, a.totalHours, a.doorLocation, a.exceptionType
                FROM AttendanceLogs a
                LEFT JOIN Employees e ON e.id = a.employeeId
                WHERE a.employeeId = @EmployeeId
                ORDER BY a.clockInTime DESC";
            return await connection.QueryAsync<Models.DTOs.AttendanceLogWithEmployeeDto>(sql, new { EmployeeId = employeeId });
        }

        public async Task<string> AddAsync(Models.AttendanceLog attendanceLog)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO AttendanceLogs (id, employeeId, clockInTime, clockOutTime, totalHours, doorLocation, exceptionType)
                VALUES (@Id, @EmployeeId, @ClockInTime, @ClockOutTime, @TotalHours, @DoorLocation, @ExceptionType)";

            var parameters = new
            {
                Id = newId.ToString(),
                EmployeeId = attendanceLog.EmployeeId,
                ClockInTime = attendanceLog.ClockInTime,
                ClockOutTime = attendanceLog.ClockOutTime,
                TotalHours = attendanceLog.TotalHours,

            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<Models.AttendanceLog> UpdateAsync(string id, Models.AttendanceLog attendanceLog)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE AttendanceLogs SET 
                employeeId = @EmployeeId,
                clockInTime = @ClockInTime,
                clockOutTime = @ClockOutTime,
                totalHours = @TotalHours,
                doorLocation = @DoorLocation,
                exceptionType = @ExceptionType
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                EmployeeId = attendanceLog.EmployeeId,
                ClockInTime = attendanceLog.ClockInTime,
                ClockOutTime = attendanceLog.ClockOutTime,
                TotalHours = attendanceLog.TotalHours,

            };

            await connection.ExecuteAsync(sql, parameters);
            return attendanceLog;
        }
    }
}
