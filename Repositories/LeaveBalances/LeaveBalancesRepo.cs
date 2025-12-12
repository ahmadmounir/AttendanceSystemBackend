using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.LeaveBalances
{
    public class LeaveBalancesRepo : ILeaveBalancesRepo
    {
        private readonly string _connectionString;

        public LeaveBalancesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.LeaveBalance>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances";
            return await connection.QueryAsync<Models.LeaveBalance>(sql);
        }

        public async Task<IEnumerable<Models.LeaveBalance>> GetEmployeeBalancesAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances WHERE employeeId = @EmployeeId";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.LeaveBalance>(sql, parameters);
        }

        public async Task<Models.LeaveBalance?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveBalance>(sql, parameters);
        }

        public async Task<Models.LeaveBalance?> GetByEmployeeAndTypeAsync(string employeeId, string leaveTypeId, int year)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances WHERE employeeId = @EmployeeId AND leaveTypeId = @LeaveTypeId AND year = @Year";
            var parameters = new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, Year = year };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveBalance>(sql, parameters);
        }

        public async Task<bool> DeductLeaveBalanceAsync(string employeeId, string leaveTypeId, int year, decimal days)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveBalances SET 
                remainingDays = remainingDays - @Days
                WHERE employeeId = @EmployeeId AND leaveTypeId = @LeaveTypeId AND year = @Year AND remainingDays >= @Days";

            var parameters = new
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Year = year,
                Days = days
            };

            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }
    }
}
