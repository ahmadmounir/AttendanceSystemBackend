using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.Notifications
{
    public class NotificationsRepo : INotificationsRepo
    {
        private readonly string _connectionString;

        public NotificationsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is required");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Notification>> GetByEmployeeIdAsync(string employeeId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Notifications WHERE employeeId = @EmployeeId ORDER BY createdAt DESC";
            var parameters = new { EmployeeId = employeeId };
            return await connection.QueryAsync<Models.Notification>(sql, parameters);
        }

       
        public async Task<int> AddAsync(Models.Notification notification)
        {
            using var connection = CreateConnection();

            var sql = @"INSERT INTO Notifications (title, descr, employeeId, markedAsRead, createdAt)
                VALUES (@Title, @Description, @EmployeeId, @MarkedAsRead, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new
            {
                Title = notification.Title,
                Description = notification.Description,
                EmployeeId = notification.EmployeeId,
                MarkedAsRead = notification.MarkedAsRead,
                CreatedAt = notification.CreatedAt
            };

            return await connection.QuerySingleAsync<int>(sql, parameters);
        }

                public async Task<int> MarkAsReadAsync(string id)
        {
            using var connection = CreateConnection();

            var sql = "UPDATE Notifications SET markedAsRead = 1 WHERE id = @Id";
            var parameters = new { Id = id };

            return await connection.ExecuteAsync(sql, parameters);
        }
    }
}
