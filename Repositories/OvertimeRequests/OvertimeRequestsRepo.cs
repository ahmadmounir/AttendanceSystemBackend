using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.OvertimeRequests
{
    public class OvertimeRequestsRepo : IOvertimeRequestsRepo
    {
        private readonly string _connectionString;

        public OvertimeRequestsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.OvertimeRequest>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT TOP 100 * FROM OverTimeRequests ORDER BY requestDate DESC";
            return await connection.QueryAsync<Models.OvertimeRequest>(sql);
        }

        public async Task<IEnumerable<Models.DTOs.OvertimeRequestWithEmployeeDto>> GetAllWithEmployeeAsync()
        {
            using var connection = CreateConnection();
            var sql = @"SELECT TOP 100
                ot.id,
                ot.employeeId,
                CONCAT(e.firstName, ' ', e.lastName) AS employeeName,
                ot.requestDate,
                ot.hours,
                ot.reason,
                ot.status
            FROM OverTimeRequests ot
            LEFT JOIN Employees e ON ot.employeeId = e.id
            ORDER BY ot.requestDate DESC";
            return await connection.QueryAsync<Models.DTOs.OvertimeRequestWithEmployeeDto>(sql);
        }

        public async Task<IEnumerable<Models.OvertimeRequest>> GetByEmployeeIdAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM OverTimeRequests WHERE employeeId = @EmployeeId ORDER BY requestDate DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.OvertimeRequest>(sql, parameters);
        }

        public async Task<Models.OvertimeRequest?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM OverTimeRequests WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.OvertimeRequest>(sql, parameters);
        }

        public async Task<string> AddAsync(Models.OvertimeRequest request)
        {
            var newId = Guid.NewGuid();
            using var connection = CreateConnection();

            // Ensure default approval status
            var status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status;

var sql = @"INSERT INTO OverTimeRequests (id, employeeId, requestDate, hours, reason, status)
                        VALUES (@Id, @EmployeeId, @RequestDate, @Hours, @Reason, @Status)";

            var parameters = new
            {
                Id = newId,
                EmployeeId = request.EmployeeId,
                RequestDate = request.RequestDate,
                Hours = request.Hours,
                Reason = request.Reason,
                Status = status
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<int> UpdateAsync(Models.OvertimeRequest request)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE OverTimeRequests SET 
                employeeId = @EmployeeId,
                requestDate = @RequestDate,
                hours = @Hours,
                reason = @Reason
                WHERE id = @Id AND status = 'Pending'";

            var parameters = new
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                RequestDate = request.RequestDate,
                Hours = request.Hours,
                Reason = request.Reason
            };

            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateApprovalStatusAsync(string id, string status)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE OverTimeRequests SET status = @Status WHERE id = @Id";
            var parameters = new { Id = id, Status = status };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM OverTimeRequests WHERE id = @Id AND status = 'Pending'";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> GetPendingCountAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT COUNT(*) FROM OverTimeRequests WHERE status = 'Pending'";
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
