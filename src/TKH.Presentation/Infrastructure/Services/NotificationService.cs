using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;
using TKH.Entities.Enums;

namespace TKH.Presentation.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ToastKey = "TKH_Notification_Key";

        public NotificationService(ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpContextAccessor httpContextAccessor)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private void PrepareTempData(NotificationType type, string message, string title)
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null) return;

            ITempDataDictionary tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

            List<NotificationMessage> notificationMessages = new List<NotificationMessage>();

            if (tempData.ContainsKey(ToastKey) && tempData[ToastKey] is string jsonStr)
            {
                try
                {
                    List<NotificationMessage> existingNotificationMessagges = JsonSerializer.Deserialize<List<NotificationMessage>>(jsonStr);
                    if (existingNotificationMessagges is not null) notificationMessages.AddRange(existingNotificationMessagges);
                }
                catch { }
            }

            notificationMessages.Add(new NotificationMessage
            {
                Type = type,
                Message = message,
                Title = title
            });

            tempData[ToastKey] = JsonSerializer.Serialize(notificationMessages);
        }

        public void Notification(NotificationType type, string message, string title = "")
            => PrepareTempData(type, message, title);

        public void Success(string message, string title = "Başarılı")
            => PrepareTempData(NotificationType.Success, message, title);

        public void Info(string message, string title = "Bilgi")
            => PrepareTempData(NotificationType.Info, message, title);

        public void Warning(string message, string title = "Uyarı")
            => PrepareTempData(NotificationType.Warning, message, title);

        public void Error(string message, string title = "Hata")
            => PrepareTempData(NotificationType.Error, message, title);

        private class NotificationMessage
        {
            public NotificationType Type { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
