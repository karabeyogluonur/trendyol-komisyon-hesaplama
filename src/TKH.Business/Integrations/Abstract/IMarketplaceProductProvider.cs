using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Dtos;

namespace TKH.Business.Integrations.Abstract
{
    public interface IMarketplaceProductProvider
    {
        IAsyncEnumerable<MarketplaceProductDto> GetProductsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
