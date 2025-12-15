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

        public async Task<Models.Notification?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Notifications WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QuerySingleOrDefaultAsync<Models.Notification>(sql, parameters);
        }

        public async Task<int> AddAsync(Models.Notification notification)
        {
            using var connection = CreateConnection();

            var newId = Guid.NewGuid().ToString();

            var sql = @"INSERT INTO Notifications (id, title, descr, employeeId, markedAsRead, createdAt)
                VALUES (@Id, @Title, @Description, @EmployeeId, @MarkedAsRead, @CreatedAt)";

            var parameters = new
            {
                Id = newId,
                Title = notification.Title,
                Description = notification.Description,
                EmployeeId = notification.EmployeeId,
                MarkedAsRead = notification.MarkedAsRead,
                CreatedAt = notification.CreatedAt
            };

            await connection.ExecuteAsync(sql, parameters);
            notification.Id = newId;
            return 1;
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
