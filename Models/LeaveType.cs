namespace AttendanceSystemBackend.Models
{
    public class LeaveType
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
        public bool IsPaid { get; set; }
        public int MaxDaysPerYear { get; set; }
    }
}
