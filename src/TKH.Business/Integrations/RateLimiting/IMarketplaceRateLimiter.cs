using System.Threading.RateLimiting;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.RateLimiting
{
    public interface IMarketplaceRateLimiter
    {
        Task<RateLimitLease> AcquireAsync(MarketplaceType marketplaceType, CancellationToken cancellationToken);
    }
}
