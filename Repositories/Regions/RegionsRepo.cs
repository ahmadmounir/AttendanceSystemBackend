using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Regions
{
    public class RegionsRepo : IRegionsRepo
    {
        private readonly string _connectionString;

        public RegionsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Region>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Regions";
            return await connection.QueryAsync<Models.Region>(sql);
        }

        public async Task<Models.Region?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Regions WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Region>(sql, parameters);
        }
    }
}
