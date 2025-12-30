using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceClaimProvider
    {
        MarketplaceType MarketplaceType { get; }
        IAsyncEnumerable<MarketplaceClaimDto> GetClaimsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
