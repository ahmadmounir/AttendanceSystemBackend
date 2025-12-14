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

        public async Task<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>> GetAllWithDetailsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT TOP (100) lr.id, lr.employeeId, (e.firstName + ' ' + e.lastName) AS employeeName,
                lr.leaveTypeId, lt.typeName AS typeName, lr.startDate, lr.endDate, lr.status, lr.reason
                FROM LeaveRequests lr
                LEFT JOIN Employees e ON e.id = lr.employeeId
                LEFT JOIN LeaveTypes lt ON lt.id = lr.leaveTypeId
                ORDER BY lr.startDate DESC";

            return await connection.QueryAsync<Models.DTOs.LeaveRequestWithDetailsDto>(sql);
        }

        public async Task<IEnumerable<Models.LeaveRequest>> GetPendingRequestsAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests WHERE status = 'Pending' ORDER BY startDate DESC";
            return await connection.QueryAsync<Models.LeaveRequest>(sql);
        }

        public async Task<IEnumerable<Models.DTOs.LeaveRequestWithDetailsDto>> GetPendingWithDetailsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT TOP (100) lr.id, lr.employeeId, (e.firstName + ' ' + e.lastName) AS employeeName,
                lr.leaveTypeId, lt.typeName AS typeName, lr.startDate, lr.endDate, lr.status, lr.reason
                FROM LeaveRequests lr
                LEFT JOIN Employees e ON e.id = lr.employeeId
                LEFT JOIN LeaveTypes lt ON lt.id = lr.leaveTypeId
                WHERE lr.status = 'Pending'
                ORDER BY lr.startDate DESC";

            return await connection.QueryAsync<Models.DTOs.LeaveRequestWithDetailsDto>(sql);
        }

        public async Task<IEnumerable<Models.LeaveRequest>> GetEmployeeRequestsAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveRequests WHERE employeeId = @EmployeeId ORDER BY startDate DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.LeaveRequest>(sql, parameters);
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
