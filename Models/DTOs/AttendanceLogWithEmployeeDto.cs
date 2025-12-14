using System;

namespace AttendanceSystemBackend.Models.DTOs
{
    public class AttendanceLogWithEmployeeDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
        public decimal? TotalHours { get; set; }
    }
}
