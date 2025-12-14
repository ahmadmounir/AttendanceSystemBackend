namespace AttendanceSystemBackend.Models.DTOs
{
    public class OvertimeRequestCreateDto
    {
        public DateTime RequestDate { get; set; }
        public decimal Hours { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }
}
