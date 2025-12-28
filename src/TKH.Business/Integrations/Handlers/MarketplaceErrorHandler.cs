using System.Net;
using TKH.Core.Common.Exceptions;

namespace TKH.Business.Integrations.Handlers
{
    public class MarketplaceErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                return response;

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new MarketplaceAuthException("API Yetkilendirme Hatası! Lütfen API Key ve Şifrenizi kontrol ediniz.");

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                throw new MarketplaceTransientException("Pazaryeri hız sınırına takıldık (Rate Limit).");

            if ((int)response.StatusCode >= 500)
                throw new MarketplaceTransientException($"Pazaryeri sunucu hatası: {response.StatusCode}");

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new MarketplaceFatalException($"API İsteği Başarısız: {response.StatusCode} - Detay: {errorBody}");
        }
    }
}
