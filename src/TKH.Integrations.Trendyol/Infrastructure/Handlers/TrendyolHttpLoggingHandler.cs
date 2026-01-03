using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TKH.Integrations.Trendyol.Infrastructure.Handlers
{
    public class TrendyolHttpLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<TrendyolHttpLoggingHandler> _logger;
        private const int MaxLogLength = 1000;

        public TrendyolHttpLoggingHandler(ILogger<TrendyolHttpLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString();

            Stopwatch stopwatch = Stopwatch.StartNew();

            string requestBody = await ReadContentSafeAsync(request.Content, cancellationToken);

            _logger.LogDebug("Trendyol Req [{CorrelationId}] -> {Method} {Url} Body: {Body}", correlationId, request.Method, request.RequestUri, requestBody);

            HttpResponseMessage response = null!;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Trendyol Req [{CorrelationId}] -> Exception after {Elapsed}ms", correlationId, stopwatch.ElapsedMilliseconds);
                throw;
            }

            stopwatch.Stop();

            if (response.Content is not null)
                await response.Content.LoadIntoBufferAsync();

            string responseBody = await ReadContentSafeAsync(response.Content, cancellationToken);

            if (response.IsSuccessStatusCode)
                _logger.LogDebug("Trendyol Res [{CorrelationId}] <- {StatusCode} ({Elapsed}ms) Body: {Body}", correlationId, (int)response.StatusCode, stopwatch.ElapsedMilliseconds, responseBody);
            else
                _logger.LogWarning("Trendyol Res [{CorrelationId}] <- Failed {StatusCode} ({Elapsed}ms) Body: {Body}", correlationId, (int)response.StatusCode, stopwatch.ElapsedMilliseconds, responseBody);

            return response;
        }

        private async Task<string> ReadContentSafeAsync(HttpContent? content, CancellationToken cancellationToken)
        {
            if (content is null) return "[Empty]";

            try
            {
                if (content.Headers.ContentType != null && !IsTextBasedContentType(content.Headers.ContentType.MediaType))
                    return $"[Binary Content: {content.Headers.ContentType.MediaType}]";

                string raw = await content.ReadAsStringAsync(cancellationToken);

                if (raw.Length > MaxLogLength)
                    return $"{raw[..MaxLogLength]}... [Truncated]";

                return raw;
            }
            catch (Exception ex)
            {
                return $"[Read Error: {ex.Message}]";
            }
        }

        private bool IsTextBasedContentType(string? mediaType)
        {
            if (string.IsNullOrEmpty(mediaType)) return false;

            return mediaType.Contains("json") ||
                   mediaType.Contains("xml") ||
                   mediaType.Contains("text") ||
                   mediaType.Contains("html") ||
                   mediaType.Contains("form-urlencoded");
        }
    }
}
