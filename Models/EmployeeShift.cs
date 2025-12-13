namespace AttendanceSystemBackend.Models
{
    public class EmployeeShift
    {
        public string EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public string ShiftId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
