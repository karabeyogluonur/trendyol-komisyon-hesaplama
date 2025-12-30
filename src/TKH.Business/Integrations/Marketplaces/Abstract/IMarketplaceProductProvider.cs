using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceProductProvider
    {
        MarketplaceType MarketplaceType { get; }
        IAsyncEnumerable<MarketplaceProductDto> GetProductsStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
