using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Shifts
{
    public class ShiftsRepo : IShiftsRepo
    {
        private readonly string _connectionString;

        public ShiftsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Shift>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Shifts";
            return await connection.QueryAsync<Models.Shift>(sql);
        }

        public async Task<Models.Shift?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Shifts WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Shift>(sql, parameters);
        }
    }
}
