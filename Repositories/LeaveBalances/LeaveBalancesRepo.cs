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

        public async Task<Models.LeaveBalance?> GetByEmployeeAndTypeAsync(string employeeId, string leaveTypeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances WHERE employeeId = @EmployeeId AND leaveTypeId = @LeaveTypeId";
            var parameters = new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveBalance>(sql, parameters);
        }

        public async Task<bool> DeductLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveBalances SET 
                remainingDays = remainingDays - @Days
                WHERE employeeId = @EmployeeId AND leaveTypeId = @LeaveTypeId AND remainingDays >= @Days";
            var parameters = new
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Days = days
            };

            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<string> AddAsync(Models.LeaveBalance leaveBalance)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO LeaveBalances (id, employeeId, leaveTypeId, remainingDays)
                        VALUES (@Id, @EmployeeId, @LeaveTypeId, @RemainingDays)";
            var parameters = new
            {
                Id = leaveBalance.Id,
                EmployeeId = leaveBalance.EmployeeId,
                LeaveTypeId = leaveBalance.LeaveTypeId,
                RemainingDays = leaveBalance.RemainingDays
            };

            await connection.ExecuteAsync(sql, parameters);
            return leaveBalance.Id;
        }

        public async Task<bool> AdjustLeaveBalanceAsync(string employeeId, string leaveTypeId, decimal days)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveBalances SET 
                remainingDays = remainingDays - @Days
                WHERE employeeId = @EmployeeId AND leaveTypeId = @LeaveTypeId";
            var parameters = new
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Days = days
            };

            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }
    }
}
