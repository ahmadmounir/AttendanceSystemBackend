using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.ViolationTypes
{
    public class ViolationTypesRepo : IViolationTypesRepo
    {
        private readonly string _connectionString;

        public ViolationTypesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.ViolationType>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM ViolationTypes";
            return await connection.QueryAsync<Models.ViolationType>(sql);
        }

        public async Task<Models.ViolationType?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM ViolationTypes WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.ViolationType>(sql, parameters);
        }
    }
}
