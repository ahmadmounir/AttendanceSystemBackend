namespace AttendanceSystemBackend.Models.DTOs
{
    public class LoginResponse
    {
        public string EmployeeId { get; set; }
        public string AccessToken { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
