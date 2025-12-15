namespace AttendanceSystemBackend.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EmployeeId { get; set; }
        public bool MarkedAsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
