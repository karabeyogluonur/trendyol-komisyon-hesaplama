using System.Net;
using System.Threading.RateLimiting;
using TKH.Business.Integrations.Marketplaces.RateLimiting;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Handlers
{
    public class MarketplaceRateLimitHandler : DelegatingHandler
    {
        private readonly IMarketplaceRateLimiter _rateLimiter;
        private readonly MarketplaceType _marketplaceType;

        public MarketplaceRateLimitHandler(
            IMarketplaceRateLimiter rateLimiter,
            MarketplaceType marketplaceType)
        {
            _rateLimiter = rateLimiter;
            _marketplaceType = marketplaceType;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using RateLimitLease lease = await _rateLimiter.AcquireAsync(_marketplaceType, cancellationToken);

            if (lease.IsAcquired)
                return await base.SendAsync(request, cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                ReasonPhrase = "Client Side Rate Limit Exceeded (Queue Full)",
                RequestMessage = request
            };
        }
    }
}

