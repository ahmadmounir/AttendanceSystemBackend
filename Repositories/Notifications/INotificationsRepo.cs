namespace AttendanceSystemBackend.Repositories.Notifications
{
    public interface INotificationsRepo
    {
        Task<IEnumerable<Models.Notification>> GetByEmployeeIdAsync(string employeeId);
        
        Task<int> AddAsync(Models.Notification notification);
        Task<int> MarkAsReadAsync(string id);
    }
}
