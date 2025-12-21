using TKH.Business.Dtos.MarketplaceAccount;

namespace TKH.Presentation.Models.MarketplaceAccount
{
    public class MarketplaceAccountSelectorViewModel
    {
        public List<MarketplaceAccountSelectorItemViewModel> MarketplaceAccounts { get; set; } = new();
        public int? CurrentMarketplaceAccountId { get; set; }
        public MarketplaceAccountSelectorItemViewModel? CurrentMarketplaceAccount { get; set; }
    }
}
