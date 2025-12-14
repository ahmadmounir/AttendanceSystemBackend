namespace AttendanceSystemBackend.Models
{
    public class LeaveBalance
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string LeaveTypeId { get; set; }
        public decimal RemainingDays { get; set; }
    }
}