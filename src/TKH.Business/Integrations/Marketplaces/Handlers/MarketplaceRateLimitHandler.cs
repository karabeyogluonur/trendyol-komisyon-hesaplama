using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;
using TKH.Business.Integrations.Marketplaces.RateLimiting;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Handlers
{
    public class MarketplaceRateLimitHandler : DelegatingHandler
    {
        private readonly IMarketplaceRateLimiter _rateLimiter;
        private readonly MarketplaceType _marketplaceType;
        private readonly ILogger<MarketplaceRateLimitHandler> _logger;

        public MarketplaceRateLimitHandler(
            IMarketplaceRateLimiter rateLimiter,
            MarketplaceType marketplaceType,
            ILogger<MarketplaceRateLimitHandler> logger)
        {
            _rateLimiter = rateLimiter;
            _marketplaceType = marketplaceType;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string? requestUri = request.RequestUri?.ToString();

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using RateLimitLease lease = await _rateLimiter.AcquireAsync(_marketplaceType, cancellationToken);
                stopwatch.Stop();

                if (lease.IsAcquired)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("RateLimit lease acquired. Marketplace: {MarketplaceType}, Waited: {ElapsedMs}ms, URL: {Url}", _marketplaceType, stopwatch.ElapsedMilliseconds, requestUri);

                    return await base.SendAsync(request, cancellationToken);
                }

                _logger.LogWarning("Client-side rate limit exceeded (Queue Full). Request rejected. Marketplace: {MarketplaceType}, URL: {Url}", _marketplaceType, requestUri);

                return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    ReasonPhrase = "Client Side Rate Limit Exceeded (Queue Full)",
                    RequestMessage = request
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while acquiring RateLimit lease. Marketplace: {MarketplaceType}", _marketplaceType);
                throw;
            }
        }
    }
}
