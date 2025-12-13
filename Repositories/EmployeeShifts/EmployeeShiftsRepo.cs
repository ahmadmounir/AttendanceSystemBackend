using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.EmployeeShifts
{
    public class EmployeeShiftsRepo : IEmployeeShiftsRepo
    {
        private readonly string _connectionString;

        public EmployeeShiftsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.EmployeeShift>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM EmployeeShifts";
            return await connection.QueryAsync<Models.EmployeeShift>(sql);
        }

        public async Task<Models.EmployeeShift?> GetByEmployeeAndStartDateAsync(string employeeId, DateTime startDate)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM EmployeeShifts WHERE employeeId = @EmployeeId AND startDate = @StartDate";
            var parameters = new { EmployeeId = employeeId, StartDate = startDate };
            return await connection.QueryFirstOrDefaultAsync<Models.EmployeeShift>(sql, parameters);
        }

        public async Task<IEnumerable<Models.EmployeeShift>> GetByEmployeeIdAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM EmployeeShifts WHERE employeeId = @EmployeeId";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.EmployeeShift>(sql, parameters);
        }

        public async Task AddAsync(Models.EmployeeShift employeeShift)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO EmployeeShifts (employeeId, startDate, shiftId, endDate) 
                VALUES (@EmployeeId, @StartDate, @ShiftId, @EndDate)";

            var parameters = new
            {
                EmployeeId = employeeShift.EmployeeId,
                StartDate = employeeShift.StartDate,
                ShiftId = employeeShift.ShiftId,
                EndDate = employeeShift.EndDate
            };

            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateAsync(string employeeId, DateTime startDate, Models.EmployeeShift employeeShift)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE EmployeeShifts 
                SET shiftId = @ShiftId, endDate = @EndDate 
                WHERE employeeId = @EmployeeId AND startDate = @StartDate";

            var parameters = new
            {
                EmployeeId = employeeId,
                StartDate = startDate,
                ShiftId = employeeShift.ShiftId,
                EndDate = employeeShift.EndDate
            };

            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<int> DeleteAsync(string employeeId, DateTime startDate)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM EmployeeShifts WHERE employeeId = @EmployeeId AND startDate = @StartDate";
            var parameters = new { EmployeeId = employeeId, StartDate = startDate };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
