using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.LeaveRequests
{
    public class LeaveRequestsRepo : ILeaveRequestsRepo
    {
        private readonly string _connectionString;

        public LeaveRequestsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.LeaveRequest>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests";
            return await connection.QueryAsync<Models.LeaveRequest>(sql);
        }

        public async Task<Models.LeaveRequest?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveRequest>(sql, parameters);
        }

        public async Task<string> AddAsync(Models.LeaveRequest leaveRequest)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO LeaveRequests (id, employeeId, leaveTypeId, startDate, endDate, reason, status)
                VALUES (@Id, @EmployeeId, @LeaveTypeId, @StartDate, @EndDate, @Reason, @Status)";

            var parameters = new
            {
                Id = newId,
                EmployeeId = leaveRequest.EmployeeId,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<Models.LeaveRequest> UpdateAsync(string id, Models.LeaveRequest leaveRequest)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveRequests SET 
                employeeId = @EmployeeId,
                leaveTypeId = @LeaveTypeId,
                startDate = @StartDate,
                endDate = @EndDate,
                reason = @Reason,
                status = @Status
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                EmployeeId = leaveRequest.EmployeeId,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status
            };

            await connection.ExecuteAsync(sql, parameters);
            return leaveRequest;
        }

        public async Task<Models.LeaveRequest> UpdateStatusAsync(string id, string status)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveRequests SET status = @Status WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                Status = status
            };

            await connection.ExecuteAsync(sql, parameters);
            
            // Retrieve and return the updated record
            var updatedRequest = await GetByIdAsync(id);
            return updatedRequest!;
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM LeaveRequests WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
