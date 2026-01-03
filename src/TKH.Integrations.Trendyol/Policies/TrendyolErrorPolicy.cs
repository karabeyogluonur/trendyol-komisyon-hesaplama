using System.Net;
using System.Text.Json;
using Refit;
using TKH.Business.Policies;
using TKH.Core.Common.Exceptions;

namespace TKH.Integrations.Trendyol.Policies
{
    public class TrendyolErrorPolicy : IIntegrationErrorPolicy
    {
        public IntegrationException Map(Exception exception)
        {
            if (exception is ApiException apiException)
            {
                return apiException.StatusCode switch
                {
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new IntegrationAuthException("Lütfen mağaza API anahtarlarınızın doğruluğunu kontrol ediniz.", apiException),
                    HttpStatusCode.TooManyRequests => new IntegrationTransientException("Pazaryeri işlem limitine takıldık. Sistem kısa bir süre bekleyip otomatik olarak tekrar deneyecektir.", apiException),
                    >= HttpStatusCode.InternalServerError => new IntegrationTransientException("Karşı pazaryeri sunucusunda geçici bir hata oluştu. Sistem daha sonra tekrar deneyecektir.", apiException),
                    _ => new IntegrationFatalException("İşlem pazaryeri tarafından reddedildi.", apiException)
                };
            }

            if (exception is JsonException)
                return new IntegrationFatalException("Pazaryeri sisteminden beklenen formatta bir yanıt alınamadı. Lütfen daha sonra tekrar deneyiniz.", exception);

            return new IntegrationFatalException("Trendyol entegrasyonu sırasında beklenmeyen bir hata oluştu.", exception);
        }

        public IntegrationException Map<T>(IApiResponse<T> response)
        {
            if (!response.IsSuccessful)
            {
                if (response.Error is null && response.Content is null)
                    return new IntegrationFatalException("Pazaryeri sisteminden beklenen formatta bir yanıt alınamadı. Lütfen daha sonra tekrar deneyiniz.");

                if (response.Error is not null)
                    return Map(response.Error);
            }

            return new IntegrationFatalException("Pazaryeri tarafından başarısız bir yanıt döndürüldü.");
        }
    }
}
