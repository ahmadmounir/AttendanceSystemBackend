namespace AttendanceSystemBackend.Models
{
    public class Violation
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public DateOnly ViolationDate { get; set; }
        public string ViolationTypeId { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public string? Notes { get; set; }
    }
}
