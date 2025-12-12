namespace AttendanceSystemBackend.Models
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
    }
}
