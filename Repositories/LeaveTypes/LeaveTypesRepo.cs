using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.LeaveTypes
{
    public class LeaveTypesRepo : ILeaveTypesRepo
    {
        private readonly string _connectionString;

        public LeaveTypesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.LeaveType>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveTypes";
            return await connection.QueryAsync<Models.LeaveType>(sql);
        }

        public async Task<Models.LeaveType?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM LeaveTypes WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.LeaveType>(sql, parameters);
        }
    }
}
