using System.Threading.RateLimiting;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.RateLimiting
{
    public interface IMarketplaceRateLimiter
    {
        Task<RateLimitLease> AcquireAsync(MarketplaceType marketplaceType, CancellationToken cancellationToken);
    }
}
