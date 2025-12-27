using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Dtos;

namespace TKH.Business.Integrations.Abstract
{
    public interface IMarketplaceClaimProvider
    {
        IAsyncEnumerable<MarketplaceClaimDto> GetClaimsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
