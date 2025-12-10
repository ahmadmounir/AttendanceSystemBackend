using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Countries
{
    public class CountriesRepo : ICountriesRepo
    {
        private readonly string _connectionString;

        public CountriesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Country>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Countries";
            return await connection.QueryAsync<Models.Country>(sql);
        }

        public async Task<Models.Country?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Countries WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Country>(sql, parameters);
        }
    }
}
