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
    a.totalHours
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
                a.clockInTime, a.clockOutTime, a.totalHours
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
                a.clockInTime, a.clockOutTime, a.totalHours
                FROM AttendanceLogs a
                LEFT JOIN Employees e ON e.id = a.employeeId
                WHERE a.employeeId = @EmployeeId
                ORDER BY a.clockInTime DESC";
            return await connection.QueryAsync<Models.DTOs.AttendanceLogWithEmployeeDto>(sql, new { EmployeeId = employeeId });
        }

        public async Task<string> AddAsync(string empId)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO AttendanceLogs (id, employeeId, clockInTime, clockOutTime, totalHours)
                VALUES (@Id, @EmployeeId, @ClockInTime, @ClockOutTime, @TotalHours)";

            var parameters = new
            {
                Id = newId.ToString(),
                EmployeeId = empId,
                ClockInTime = DateTime.Now,
                ClockOutTime = (DateTime?)null,
                TotalHours = 0,
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<string> UpdateAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE AttendanceLogs SET
                clockOutTime = @ClockOutTime,
                totalHours = @TotalHours
                WHERE id = @Id";

            var log = await GetByIdAsync(id);
            var now = DateTime.Now;

            var duration = now - log.ClockInTime;

            var parameters = new
            {
                Id = id,
                ClockOutTime = now,
                TotalHours = (decimal)duration.TotalMinutes // for test 
            };

            await connection.ExecuteAsync(sql, parameters);
            return id;
        }
    }
}
