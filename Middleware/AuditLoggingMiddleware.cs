using AttendanceSystemBackend.Repositories.AuditLogs;
using System.Security.Claims;

namespace AttendanceSystemBackend.Middleware
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogsRepo auditLogsRepo)
        {
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method;

            // Skip logging for auth endpoints and non-API requests
            if (path == null || 
                path.Contains("/auth/") || 
                !path.StartsWith("/api/"))
            {
                await _next(context);
                return;
            }

            // Only log if user is authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                ?? context.User.FindFirst("sub")?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        var tableName = ExtractTableNameFromPath(path);
                        var action = MapHttpMethodToAction(method);

                        var auditLog = new Models.AuditLog
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userId,
                            Action = action,
                            TableName = tableName,
                            Timestamp = DateTime.UtcNow
                        };

                        await auditLogsRepo.CreateAuditLogAsync(auditLog);
                        
                        _logger.LogInformation(
                            "Audit Log: User {UserId} performed {Action} on {TableName} at {Timestamp}",
                            userId, action, tableName, auditLog.Timestamp);
                    }
                }
                catch (Exception ex)
                {
                    // Don't fail the request if audit logging fails
                    _logger.LogError(ex, "Failed to create audit log");
                }
            }

            await _next(context);
        }

        private string ExtractTableNameFromPath(string path)
        {
            // Remove /api/v1/ prefix and get the first segment
            var segments = path.Replace("/api/v1/", "").Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length > 0 ? segments[0] : "Unknown";
        }

        private string MapHttpMethodToAction(string method)
        {
            return method.ToUpper() switch
            {
                "GET" => "READ",
                "POST" => "CREATE",
                "PUT" => "UPDATE",
                "DELETE" => "DELETE",
                "PATCH" => "UPDATE",
                _ => method.ToUpper()
            };
        }
    }
}
