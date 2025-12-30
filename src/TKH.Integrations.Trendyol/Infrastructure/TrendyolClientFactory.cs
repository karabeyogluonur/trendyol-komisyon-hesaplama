using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;
using TKH.Business.Integrations.Providers.Trendyol;

namespace TKH.Integrations.Trendyol.Infrastructure
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
