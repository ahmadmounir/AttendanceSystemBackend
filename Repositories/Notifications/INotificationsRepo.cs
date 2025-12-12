namespace AttendanceSystemBackend.Repositories.Notifications
{
    public interface INotificationsRepo
    {
        Task<IEnumerable<Models.Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Models.Notification>> GetUnreadNotificationsAsync(string userId);
        Task<Models.Notification?> GetByIdAsync(string id);
        Task<string> CreateNotificationAsync(Models.Notification notification);
        Task<bool> MarkAsReadAsync(string id);
        Task<bool> MarkAllAsReadAsync(string userId);
    }
}
