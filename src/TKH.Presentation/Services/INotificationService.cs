using TKH.Entities.Enums;

namespace TKH.Presentation.Services
{
    public interface INotificationService
    {
        void Notification(NotificationType type, string message, string title = "");
        void Success(string message, string title = "");
        void Info(string message, string title = "");
        void Warning(string message, string title = "");
        void Error(string message, string title = "");
    }
}
