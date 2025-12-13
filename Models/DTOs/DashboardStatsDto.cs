namespace AttendanceSystemBackend.Models.DTOs
{
    public class DashboardStatsDto
    {
        public int PendingOvertimeRequests { get; set; }
        public int PendingLeaveRequests { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalEmployees { get; set; }
    }
}
