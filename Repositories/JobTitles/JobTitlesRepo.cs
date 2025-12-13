using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.JobTitles
{
    public class JobTitlesRepo : IJobTitlesRepo
    {
        private readonly string _connectionString;

        public JobTitlesRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.JobTitle>> GetAllAsync()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM jobTitles";
            return await connection.QueryAsync<Models.JobTitle>(sql);
        }

        public async Task<Models.JobTitle?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM jobTitles WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.JobTitle>(sql, parameters);
        }

        public async Task<string> AddAsync(string titleName, float minSalary, float maxSalary)
        {
            var newId = Guid.NewGuid();

            using var connection = CreateConnection();

            var sql = @"INSERT INTO jobTitles (id, titleName, minSalary, maxSalary)
                VALUES (@Id, @TitleName, @MinSalary, @MaxSalary)";

            var parameters = new
            {
                Id = newId,
                TitleName = titleName,
                MinSalary = minSalary,
                MaxSalary = maxSalary
            };

            await connection.ExecuteAsync(sql, parameters);
            return newId.ToString();
        }

        public async Task<Models.JobTitle> UpdateAsync(string id, string titleName, float minSalary, float maxSalary)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE jobTitles SET 
                titleName = @TitleName,
                minSalary = @MinSalary,
                maxSalary = @MaxSalary
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                TitleName = titleName,
                MinSalary = minSalary,
                MaxSalary = maxSalary
            };

            await connection.ExecuteAsync(sql, parameters);
            
            return new Models.JobTitle
            {
                Id = id,
                TitleName = titleName,
                MinSalary = minSalary,
                MaxSalary = maxSalary
            };
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM jobTitles WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
