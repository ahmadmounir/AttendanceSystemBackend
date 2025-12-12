namespace AttendanceSystemBackend.Models
{
    public class LeaveRequest
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } 
    }
}