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

        public async Task<Models.LeaveBalance?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveBalances WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveBalance>(sql, parameters);
        }

        public async Task<string> AddAsync(Models.LeaveBalance leaveBalance)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO LeaveBalances (id, employeeId, leaveTypeId, remainingDays, year)
                VALUES (@Id, @EmployeeId, @LeaveTypeId, @RemainingDays, @Year)";

            var parameters = new
            {
                Id = newId,
                EmployeeId = leaveBalance.EmployeeId,
                LeaveTypeId = leaveBalance.LeaveTypeId,
                RemainingDays = leaveBalance.RemainingDays,
                Year = leaveBalance.Year
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<Models.LeaveBalance> UpdateAsync(string id, Models.LeaveBalance leaveBalance)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveBalances SET 
                employeeId = @EmployeeId,
                leaveTypeId = @LeaveTypeId,
                remainingDays = @RemainingDays,
                year = @Year
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                EmployeeId = leaveBalance.EmployeeId,
                LeaveTypeId = leaveBalance.LeaveTypeId,
                RemainingDays = leaveBalance.RemainingDays,
                Year = leaveBalance.Year
            };

            await connection.ExecuteAsync(sql, parameters);
            return leaveBalance;
        }
    }
}
