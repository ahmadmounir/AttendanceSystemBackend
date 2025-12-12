using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Departments
{
    public class DepartmentsRepo : IDepartmentsRepo
    {
        private readonly string _connectionString;

        public DepartmentsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Department>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Departments";
            return await connection.QueryAsync<Models.Department>(sql);
        }

        public async Task<Models.Department?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Departments WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Department>(sql, parameters);
        }

        public async Task<string> AddAsync(string id, Models.Department department)
        {
            using var connection = CreateConnection();

            var sql = @"INSERT INTO Departments (id, departmentName) 
                VALUES (@Id, @DepartmentName)";

            var parameters = new
            {
                Id = id,
                DepartmentName = department.DepartmentName,
            };

            await connection.ExecuteAsync(sql, parameters);

            return id;
        }
        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Departments WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
