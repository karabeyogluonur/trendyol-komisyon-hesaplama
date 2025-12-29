using TKH.Entities.Enums;

namespace TKH.Business.Integrations.RateLimiting
{
    public sealed class MarketplaceRateLimitOptions
    {
        public MarketplaceType MarketplaceType { get; init; }
        public int PermitLimit { get; init; }
        public int QueueLimit { get; init; } = 100;
        public TimeSpan ReplenishmentPeriod { get; init; }
        public int TokensPerPeriod { get; init; }
    }
}
