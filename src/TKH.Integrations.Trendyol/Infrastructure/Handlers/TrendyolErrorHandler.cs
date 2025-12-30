using System.Net;
using Microsoft.Extensions.Logging;
using TKH.Core.Common.Exceptions;

namespace TKH.Integrations.Trendyol.Infrastructure.Handlers
{
    public class TrendyolErrorHandler : DelegatingHandler
    {
        private readonly ILogger<TrendyolErrorHandler> _logger;
        private const int MaxLogLength = 1000;

        public TrendyolErrorHandler(ILogger<TrendyolErrorHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestBodyPreview = "[Not Logged]";

            if (_logger.IsEnabled(LogLevel.Debug) || request.Content is not null)
                requestBodyPreview = await ReadContentAsync(request.Content, cancellationToken);

            string requestUri = request.RequestUri?.ToString();
            string method = request.Method.Method;

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            bool shouldLogBody = !response.IsSuccessStatusCode || _logger.IsEnabled(LogLevel.Debug);
            string responseBodyPreview = "[Not Logged]";

            if (response.Content is not null && shouldLogBody)
            {
                await response.Content.LoadIntoBufferAsync();
                responseBodyPreview = await ReadContentAsync(response.Content, cancellationToken);
            }

            if (response.IsSuccessStatusCode)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("API Success: {Method} {Url} ({StatusCode}) \nReq: {RequestBody} \nRes: {ResponseBody}",
                        method, requestUri, (int)response.StatusCode, requestBodyPreview, responseBodyPreview);
                }
                else
                {
                    _logger.LogInformation("API Success: {Method} {Url} ({StatusCode})",
                        method, requestUri, (int)response.StatusCode);
                }

                return response;
            }

            _logger.LogError("API Failure: {Method} {Url} ({StatusCode}) \nReq: {RequestBody} \nRes: {ResponseBody}",
                method, requestUri, (int)response.StatusCode, requestBodyPreview, responseBodyPreview);

            await HandleErrorAsync(response);

            return response;
        }

        private async Task HandleErrorAsync(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;

            if (statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new MarketplaceAuthException("Lütfen mağaza API anahtarlarınızın doğruluğunu kontrol ediniz.");

            if (statusCode is HttpStatusCode.TooManyRequests)
                throw new MarketplaceTransientException("Pazaryeri işlem limitine takıldık. Sistem kısa bir süre bekleyip işlemi otomatik olarak tekrar deneyecektir.");

            if ((int)statusCode >= 500)
                throw new MarketplaceTransientException($"Karşı pazaryeri sunucusunda geçici bir hata oluştu ({statusCode}). Sistem daha sonra tekrar deneyecektir.");

            throw new MarketplaceFatalException($"İşlem pazaryeri tarafından reddedildi ({statusCode}).");
        }

        private async Task<string> ReadContentAsync(HttpContent? content, CancellationToken cancellationToken)
        {
            if (content is null) return "[Empty]";

            try
            {
                string contentString = await content.ReadAsStringAsync(cancellationToken);
                return Truncate(contentString);
            }
            catch (Exception ex)
            {
                return $"[Error Reading Body: {ex.Message}]";
            }
        }

        private static string Truncate(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= MaxLogLength ? value : $"{value.Substring(0, MaxLogLength)}... [Truncated]";
        }
    }
}
