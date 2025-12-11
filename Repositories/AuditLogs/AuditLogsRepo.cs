using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AttendanceSystemBackend.Repositories.AuditLogs
{
    public class AuditLogsRepo : IAuditLogsRepo
    {
        private readonly string _connectionString;

        public AuditLogsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<string> CreateAuditLogAsync(Models.AuditLog auditLog)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO AuditLogs (id, userId, action, tableName, timestamp) 
                        VALUES (@Id, @UserId, @Action, @TableName, @Timestamp)";
            
            var parameters = new
            {
                Id = auditLog.Id,
                UserId = auditLog.UserId,
                Action = auditLog.Action,
                TableName = auditLog.TableName,
                Timestamp = auditLog.Timestamp
            };

            await connection.ExecuteAsync(sql, parameters);
            return auditLog.Id;
        }
    }
}
