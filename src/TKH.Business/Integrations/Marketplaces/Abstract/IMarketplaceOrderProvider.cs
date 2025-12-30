using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Marketplaces.Abstract
{
    public interface IMarketplaceOrderProvider
    {
        MarketplaceType MarketplaceType { get; }
        IAsyncEnumerable<MarketplaceOrderDto> GetOrdersStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
