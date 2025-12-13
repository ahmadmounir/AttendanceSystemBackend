namespace AttendanceSystemBackend.Models.DTOs
{
    public class CreateEmployeeWithAccountDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ShiftId { get; set; }
        public string DepartmentId { get; set; }
        public string JobId { get; set; }
        public string CountryId { get; set; }
        public bool IsSystemActive { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
    }
}
