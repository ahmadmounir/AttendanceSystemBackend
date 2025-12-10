using AttendanceSystemBackend.Repositories.Employees;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AttendanceSystemBackend.Repositories.Employees
{
    public class EmployeesRepo : IEmployeesRepo
    {
        private readonly string _connectionString;

        public EmployeesRepo(IConfiguration configuration)
        {
            // جلب نص الاتصال من ملف appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // دالة مساعدة لإنشاء اتصال جديد عند الحاجة فقط
        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Employees>> GetAllAsync()
        {
            using var connection = CreateConnection();

            var sql = "SELECT * FROM Employees";
            return await connection.QueryAsync<Models.Employees>(sql);
        }

        public async Task<string> AddAsync(Models.Employees employee)
        {
            // 1. Generate the ID
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            // 2. The SQL query
            var sql = @"INSERT INTO Employees (id, firstName, lastName, email, phone, hireDate, startDate, shiftId, endDate, departmentId, jobId, countryId, isSystemActive) 
                VALUES (@Id, @FirstName, @LastName, @Email, @Phone, @HireDate, @StartDate, @ShiftId, @EndDate, @DepartmentId, @JobId, @CountryId, @IsSystemActive)";

            // 3. Create the parameters object
            var parameters = new
            {
                Id = newId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                HireDate = employee.HireDate,
                StartDate = employee.StartDate,
                ShiftId = employee.ShiftId,
                EndDate = employee.EndDate,
                DepartmentId = employee.DepartmentId,
                JobId = employee.JobId,
                CountryId = employee.CountryId,
                IsSystemActive = employee.IsSystemActive
            };

            // 4. Execute
            await connection.ExecuteAsync(sql, parameters);

            // 5. Return the ID created in step 1
            return newId.ToString();
        }

        public async Task<Models.Employees> UpdateAsync(string id, Models.Employees employee)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE Employees SET 
                firstName = @FirstName,
                lastName = @LastName,
                email = @Email,
                phone = @Phone,
                hireDate = @HireDate,
                startDate = @StartDate,
                endDate = @EndDate,
                shiftId = @ShiftId,
                departmentId = @DepartmentId,
                jobId = @JobId,
                countryId = @CountryId,
                isSystemActive = @IsSystemActive
                WHERE id = @Id";
            var parameters = new
            {
                Id = id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                HireDate = employee.HireDate,
                StartDate = employee.StartDate,
                EndDate = employee.EndDate,
                ShiftId = employee.ShiftId,
                DepartmentId = employee.DepartmentId,
                JobId = employee.JobId,
                CountryId = employee.CountryId,
                IsSystemActive = employee.IsSystemActive
            };
            await connection.ExecuteAsync(sql, parameters);
            return employee;
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Employees WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}