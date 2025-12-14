namespace AttendanceSystemBackend.Models.DTOs
{
    public class OvertimeRequestWithEmployeeDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Hours { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }
}
