using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Violations
{
    public class ViolationsRepo : IViolationsRepo
    {
        private readonly string _connectionString;

        public ViolationsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Violation>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Violations";
            return await connection.QueryAsync<Models.Violation>(sql);
        }

        public async Task<Models.Violation?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Violations WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Violation>(sql, parameters);
        }

        public async Task<IEnumerable<Models.Violation>> GetByEmployeeIdAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Violations WHERE employeeId = @EmployeeId";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.Violation>(sql, parameters);
        }

        public async Task<string> AddAsync(Models.Violation violation)
        {
            var newId = Guid.NewGuid().ToString();
            
            using var connection = CreateConnection();
            var sql = @"INSERT INTO Violations (id, employeeId, violationDate, violationTypeId, penaltyAmount, notes) 
                VALUES (@Id, @EmployeeId, @ViolationDate, @ViolationTypeId, @PenaltyAmount, @Notes)";

            var parameters = new
            {
                Id = newId,
                EmployeeId = violation.EmployeeId,
                ViolationDate = violation.ViolationDate,
                ViolationTypeId = violation.ViolationTypeId,
                PenaltyAmount = violation.PenaltyAmount,
                Notes = violation.Notes
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId;
        }

        public async Task<int> UpdateAsync(string id, Models.Violation violation)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE Violations 
                SET employeeId = @EmployeeId, 
                    violationDate = @ViolationDate, 
                    violationTypeId = @ViolationTypeId, 
                    penaltyAmount = @PenaltyAmount, 
                    notes = @Notes 
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                EmployeeId = violation.EmployeeId,
                ViolationDate = violation.ViolationDate,
                ViolationTypeId = violation.ViolationTypeId,
                PenaltyAmount = violation.PenaltyAmount,
                Notes = violation.Notes
            };

            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Violations WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
