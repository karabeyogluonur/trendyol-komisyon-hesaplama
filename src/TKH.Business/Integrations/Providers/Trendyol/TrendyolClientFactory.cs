using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;
using TKH.Core.Common.Exceptions;

namespace TKH.Business.Integrations.Providers.Trendyol
{
    public class TrendyolClientFactory(IHttpClientFactory httpClientFactory)
    {
        private readonly RefitSettings _refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            }),

            ExceptionFactory = async (HttpResponseMessage httpResponseMessage) =>
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                    return null;

                string errorContent = await httpResponseMessage.Content.ReadAsStringAsync();

                return httpResponseMessage.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new MarketplaceAuthException($"Trendyol Yetkilendirme Hatası (401). Lütfen API Key ve Secret kontrol ediniz."),
                    HttpStatusCode.Forbidden => new MarketplaceAuthException($"Trendyol Erişim Engellendi (403). Mağazanın bu işlemi yapmaya yetkisi yok."),
                    HttpStatusCode.RequestTimeout => new MarketplaceTransientException("Trendyol sunucusu zaman aşımına uğradı (408)."),
                    HttpStatusCode.ServiceUnavailable => new MarketplaceTransientException("Trendyol servisi şu an kullanılamıyor (503)."),
                    HttpStatusCode.GatewayTimeout => new MarketplaceTransientException("Trendyol Gateway zaman aşımı (504)."),
                    HttpStatusCode.TooManyRequests => new MarketplaceTransientException("Trendyol hız limitine takıldınız (429). Lütfen bekleyip tekrar deneyin."),
                    HttpStatusCode.BadRequest => new MarketplaceFatalException($"Hatalı İstek (400). Gönderilen veri Trendyol formatına uymuyor. Detay: {errorContent}"),
                    HttpStatusCode.InternalServerError => new MarketplaceFatalException("Trendyol Sunucu Hatası (500)"),
                    _ => new MarketplaceFatalException($"Beklenmeyen Trendyol API Hatası: {httpResponseMessage.StatusCode}")
                };
            }
        };

        public T CreateClient<T>(long sellerIdentifier, string apiKey, string apiSecret) where T : class
        {
            HttpClient httpClient = httpClientFactory.CreateClient(TrendyolDefaults.HttpClientName);
            httpClient.BaseAddress = new Uri(TrendyolDefaults.BaseUrl);

            string authenticationCredentials = $"{apiKey}:{apiSecret}";
            byte[] authenticationBytes = Encoding.UTF8.GetBytes(authenticationCredentials);
            string encodedAuthenticationHeader = Convert.ToBase64String(authenticationBytes);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuthenticationHeader);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{sellerIdentifier} - {TrendyolDefaults.UserAgentSuffix}");

            return RestService.For<T>(httpClient, _refitSettings);
        }

        public T CreatePublicClient<T>() where T : class
        {
            HttpClient httpClient = httpClientFactory.CreateClient(TrendyolDefaults.HttpClientName);
            httpClient.BaseAddress = new Uri(TrendyolDefaults.BaseUrl);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"Public - {TrendyolDefaults.UserAgentSuffix}");

            return RestService.For<T>(httpClient, _refitSettings);
        }
    }
}
