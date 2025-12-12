namespace AttendanceSystemBackend.Models
{
    public class Shift
    {
        public string Id { get; set; }
        public string ShiftName { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? GracePeriodMinutes { get; set; }
    }
}
