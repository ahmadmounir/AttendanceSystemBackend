using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using AttendanceSystemBackend.Models.DTOs;

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

        public async Task<IEnumerable<DepartmentWithEmployeeCountDto>> GetAllWithEmployeeCountAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    d.id AS Id,
                    d.departmentName AS DepartmentName,
                    COUNT(e.id) AS EmployeeCount
                FROM Departments d
                LEFT JOIN Employees e ON d.id = e.departmentId
                GROUP BY d.id, d.departmentName
                ORDER BY d.departmentName";
            
            return await connection.QueryAsync<DepartmentWithEmployeeCountDto>(sql);
        }

        public async Task<Models.Department?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Departments WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Department>(sql, parameters);
        }

        public async Task<string> AddAsync(string name)
        {
            var newId = Guid.NewGuid().ToString();
            
            using var connection = CreateConnection();

            var sql = @"INSERT INTO Departments (id, departmentName) 
                VALUES (@Id, @DepartmentName)";

            var parameters = new
            {
                Id = newId,
                DepartmentName = name,
            };

            await connection.ExecuteAsync(sql, parameters);

            return newId;
        }

        public async Task<int> UpdateAsync(string id, string name)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE Departments SET departmentName = @DepartmentName WHERE id = @Id";
            var parameters = new
            {
                Id = id,
                DepartmentName = name
            };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Departments WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT COUNT(*) FROM Departments";
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}
