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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Models.Notification>> GetUserNotificationsAsync(string userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Notifications WHERE userId = @UserId ORDER BY createdAt DESC";
            var parameters = new { UserId = userId };
            return await connection.QueryAsync<Models.Notification>(sql, parameters);
        }

        public async Task<IEnumerable<Models.Notification>> GetUnreadNotificationsAsync(string userId)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Notifications WHERE userId = @UserId AND isRead = 0 ORDER BY createdAt DESC";
            var parameters = new { UserId = userId };
            return await connection.QueryAsync<Models.Notification>(sql, parameters);
        }

        public async Task<Models.Notification?> GetByIdAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Notifications WHERE id = @Id";
            var parameters = new { Id = id };
            return await connection.QueryFirstOrDefaultAsync<Models.Notification>(sql, parameters);
        }

        public async Task<string> CreateNotificationAsync(Models.Notification notification)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO Notifications (id, userId, message, isRead, createdAt) 
                        VALUES (@Id, @UserId, @Message, @IsRead, @CreatedAt)";
            
            var parameters = new
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            await connection.ExecuteAsync(sql, parameters);
            return notification.Id;
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Notifications SET isRead = 1 WHERE id = @Id";
            var parameters = new { Id = id };
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Notifications SET isRead = 1 WHERE userId = @UserId";
            var parameters = new { UserId = userId };
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }
    }
}
