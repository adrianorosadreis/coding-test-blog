using SimpleBlog.Models;

namespace SimpleBlog.Services
{
    public interface INotificationService
    {
        void SendPostCreatedNotification(Post post);
    }
}