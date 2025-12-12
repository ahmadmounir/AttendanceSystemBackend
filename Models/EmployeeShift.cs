namespace AttendanceSystemBackend.Models
{
    public class EmployeeShift
    {
        public string EmployeeId { get; set; }
        public DateOnly StartDate { get; set; }
        public string ShiftId { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
