namespace AttendanceSystemBackend.Repositories.AuditLogs
{
    public interface IAuditLogsRepo
    {
        Task<string> CreateAuditLogAsync(Models.AuditLog auditLog);
    }
}
