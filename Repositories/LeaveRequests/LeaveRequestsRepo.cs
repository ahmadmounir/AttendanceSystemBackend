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
            var sql = "SELECT * FROM LeaveRequests ORDER BY startDate DESC";
            return await connection.QueryAsync<Models.LeaveRequest>(sql);
        }

        public async Task<IEnumerable<Models.LeaveRequest>> GetPendingRequestsAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests WHERE status = 'Pending' ORDER BY startDate DESC";
            return await connection.QueryAsync<Models.LeaveRequest>(sql);
        }

        public async Task<IEnumerable<Models.LeaveRequest>> GetEmployeeRequestsAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests WHERE employeeId = @EmployeeId ORDER BY startDate DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.LeaveRequest>(sql, parameters);
        }

        public async Task<IEnumerable<Models.DTOs.LeaveRequestWithBalanceDto>> GetEmployeeRequestsWithBalanceAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT
                lr.id,
                lr.employeeId,
                lr.leaveTypeId,
                lt.typeName,
                lr.startDate,
                lr.endDate,
                lr.status,
                lr.reason,
                ISNULL(lb.remainingDays, 0) AS remainingDays,
                lt.maxDaysPerYear
            FROM LeaveRequests lr
            LEFT JOIN LeaveTypes lt ON lr.leaveTypeId = lt.id
            LEFT JOIN LeaveBalances lb ON lr.employeeId = lb.employeeId AND lr.leaveTypeId = lb.leaveTypeId
            WHERE lr.employeeId = @EmployeeId
            ORDER BY lr.startDate DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.DTOs.LeaveRequestWithBalanceDto>(sql, parameters);
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

        public async Task<bool> ReviewRequestAsync(string id, string status, string reviewedBy, string? reviewNotes)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE LeaveRequests SET status = @Status WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                Status = status
            };

            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM LeaveRequests WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> GetPendingCountAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT COUNT(*) FROM LeaveRequests WHERE status = 'Pending'";
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
