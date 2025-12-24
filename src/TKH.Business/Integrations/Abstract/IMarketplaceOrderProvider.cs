using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Dtos;

namespace TKH.Business.Integrations.Abstract
{
    public interface IMarketplaceOrderProvider
    {
        IAsyncEnumerable<MarketplaceOrderDto> GetOrdersStreamAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto, CancellationToken cancellationToken = default);
    }
}
