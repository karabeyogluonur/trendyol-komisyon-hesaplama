using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

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
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            }),

            ExceptionFactory = async (HttpResponseMessage httpResponseMessage) =>
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                    return null;

                string errorContent = await httpResponseMessage.Content.ReadAsStringAsync();

                return httpResponseMessage.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new Exception($"Trendyol yetkilendirme hatası (401). API Key/Secret kontrol edin. Detay: {errorContent}"),
                    HttpStatusCode.Forbidden => new Exception($"Yetki hatası (403). Detay: {errorContent}"),
                    HttpStatusCode.TooManyRequests => new Exception("Trendyol API hız limitine takıldınız (429)."),
                    HttpStatusCode.BadRequest => new Exception($"Hatalı İstek (400). Detay: {errorContent}"),
                    HttpStatusCode.InternalServerError => new Exception("Trendyol Sunucu Hatası (500)."),
                    _ => new Exception($"Trendyol API Hatası: {httpResponseMessage.StatusCode}. Detay: {errorContent}")
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
