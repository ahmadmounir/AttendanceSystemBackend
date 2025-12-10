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

        public async Task<string> AddAsync(Models.Department department)
        {
            // 1. Generate the ID
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            // 2. The SQL query
            var sql = @"INSERT INTO Employees (id, departmentName) 
                VALUES (@Id, @DepartmentName)";

            // 3. Create the parameters object
            var parameters = new
            {
                Id = newId,
                DepartmentName = department.DepartmentName,
                
            };

            // 4. Execute
            await connection.ExecuteAsync(sql, parameters);

            // 5. Return the ID created in step 1
            return newId.ToString();
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
