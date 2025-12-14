namespace AttendanceSystemBackend.Models
{
    public class OvertimeRequest
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Hours { get; set; } 
        public string Reason { get; set; }
        public string Status { get; set; } 
    }
}