namespace AttendanceSystemBackend.Models
{
    public class AttendanceLog
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime ClockInTime { get; set; }

        public DateTime? ClockOutTime { get; set; }
        public decimal? TotalHours { get; set; }

    }
}
