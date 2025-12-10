using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.UserRoles
{
    public class UserRolesRepo : IUserRolesRepo
    {
        private readonly string _connectionString;

        public UserRolesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.UserRole>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM UserRoles";
            return await connection.QueryAsync<Models.UserRole>(sql);
        }

        public async Task<Models.UserRole?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM UserRoles WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.UserRole>(sql, parameters);
        }
    }
}
