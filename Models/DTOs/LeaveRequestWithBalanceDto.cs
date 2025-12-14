namespace AttendanceSystemBackend.Models.DTOs
{
    public class LeaveRequestWithBalanceDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string LeaveTypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public decimal RemainingDays { get; set; }
        public int MaxDaysPerYear { get; set; }
    }
}
