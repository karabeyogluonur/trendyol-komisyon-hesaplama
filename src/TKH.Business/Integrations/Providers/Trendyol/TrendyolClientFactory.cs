using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Refit;
using TKH.Business.Integrations.Providers.Trendyol.Services;

namespace TKH.Business.Integrations.Providers.Trendyol;

public class TrendyolClientFactory(IHttpClientFactory httpClientFactory)
{
    public ITrendyolProductService CreateClient(long sellerIdentifier, string apiKey, string apiSecret)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(TrendyolDefaults.HttpClientName);
        httpClient.BaseAddress = new Uri(TrendyolDefaults.BaseUrl);

        string authenticationCredentials = $"{apiKey}:{apiSecret}";
        byte[] authenticationBytes = Encoding.UTF8.GetBytes(authenticationCredentials);
        string encodedAuthenticationHeader = Convert.ToBase64String(authenticationBytes);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuthenticationHeader);
        httpClient.DefaultRequestHeaders.Add("User-Agent", $"{sellerIdentifier} - {TrendyolDefaults.UserAgentSuffix}");

        RefitSettings refitSettings = new RefitSettings
        {
            ExceptionFactory = async (HttpResponseMessage httpResponseMessage) =>
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                    return null;

                string errorContent = await httpResponseMessage.Content.ReadAsStringAsync();

                return httpResponseMessage.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new Exception($"Trendyol yetkilendirme hatası (401). API anahtarlarınızı kontrol edin. Detay: {errorContent}"),
                    HttpStatusCode.Forbidden => new Exception("Bu işlem için Trendyol üzerinde yetkiniz bulunmuyor (403)."),
                    HttpStatusCode.TooManyRequests => new Exception("Trendyol API limitine takıldınız (429). Lütfen bir süre bekleyin."),
                    HttpStatusCode.BadRequest => new Exception($"Hatalı istek (400). Gönderilen verileri kontrol edin. Detay: {errorContent}"),
                    HttpStatusCode.InternalServerError => new Exception("Trendyol sunucularında bir hata oluştu (500)."),
                    _ => new Exception($"Trendyol API hatası: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}. İçerik: {errorContent}")
                };
            }
        };

        return RestService.For<ITrendyolProductService>(httpClient, refitSettings);
    }
}
