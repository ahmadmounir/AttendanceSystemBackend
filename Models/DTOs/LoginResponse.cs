namespace AttendanceSystemBackend.Models.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string RoleName { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
