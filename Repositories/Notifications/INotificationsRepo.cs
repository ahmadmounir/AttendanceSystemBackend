namespace AttendanceSystemBackend.Repositories.Notifications
{
    public interface INotificationsRepo
    {
        Task<IEnumerable<Models.Notification>> GetByEmployeeIdAsync(string employeeId);
        
        Task<Models.Notification?> GetByIdAsync(string id);

        Task<int> AddAsync(Models.Notification notification);
        Task<int> MarkAsReadAsync(string id);
    }
}
