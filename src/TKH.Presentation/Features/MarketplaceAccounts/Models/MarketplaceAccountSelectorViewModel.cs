namespace TKH.Presentation.Features.MarketplaceAccounts.Models
{
    public class MarketplaceAccountSelectorViewModel
    {
        public List<MarketplaceAccountSelectorItemViewModel> MarketplaceAccounts { get; set; } = new();
        public int? CurrentMarketplaceAccountId { get; set; }
        public MarketplaceAccountSelectorItemViewModel? CurrentMarketplaceAccount { get; set; }
    }
}
