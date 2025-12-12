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

        public async Task<Models.AttendanceLog?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM AttendanceLogs WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.AttendanceLog>(sql, parameters);
        }

        public async Task<string> AddAsync(Models.AttendanceLog attendanceLog)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO AttendanceLogs (id, employeeId, clockInTime, clockOutTime, totalHours, doorLocation, exceptionType)
                VALUES (@Id, @EmployeeId, @ClockInTime, @ClockOutTime, @TotalHours, @DoorLocation, @ExceptionType)";

            var parameters = new
            {
                Id = newId,
                EmployeeId = attendanceLog.EmployeeId,
                ClockInTime = attendanceLog.ClockInTime,
                ClockOutTime = attendanceLog.ClockOutTime,
                TotalHours = attendanceLog.TotalHours,
                DoorLocation = attendanceLog.DoorLocation,
                ExceptionType = attendanceLog.ExceptionType
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
                DoorLocation = attendanceLog.DoorLocation,
                ExceptionType = attendanceLog.ExceptionType
            };

            await connection.ExecuteAsync(sql, parameters);
            return attendanceLog;
        }
    }
}
