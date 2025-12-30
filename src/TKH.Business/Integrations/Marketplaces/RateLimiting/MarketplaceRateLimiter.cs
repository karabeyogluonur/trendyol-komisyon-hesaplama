using System.Threading.RateLimiting;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.RateLimiting
{
    public class MarketplaceRateLimiter : IMarketplaceRateLimiter, IDisposable
    {
        private readonly PartitionedRateLimiter<MarketplaceType> _limiter;

        public MarketplaceRateLimiter(IEnumerable<MarketplaceRateLimitOptions> options)
        {
            _limiter = PartitionedRateLimiter.Create<MarketplaceType, MarketplaceType>(resourceKey =>
            {
                var option = options.FirstOrDefault(o => o.MarketplaceType == resourceKey);

                if (option == null)
                {
                    return RateLimitPartition.GetFixedWindowLimiter(resourceKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    });
                }

                return RateLimitPartition.GetTokenBucketLimiter(resourceKey, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = option.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = option.QueueLimit,
                    ReplenishmentPeriod = option.ReplenishmentPeriod,
                    TokensPerPeriod = option.TokensPerPeriod,
                    AutoReplenishment = true
                });
            });
        }

        public async Task<RateLimitLease> AcquireAsync(MarketplaceType marketplaceType, CancellationToken cancellationToken)
        {
            return await _limiter.AcquireAsync(marketplaceType, permitCount: 1, cancellationToken);
        }

        public void Dispose()
        {
            _limiter?.Dispose();
        }
    }
}
