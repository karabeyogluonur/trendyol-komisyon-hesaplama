using System.ComponentModel.DataAnnotations;

namespace TKH.Web.Features.MarketplaceAccounts.Models
{
    public class MarketplaceAccountSelectorViewModel
    {
        public List<MarketplaceAccountSelectorItemViewModel> MarketplaceAccounts { get; set; } = new();

        [Display(Name = "Seçili Mağaza")]
        public int? CurrentMarketplaceAccountId { get; set; }

        public MarketplaceAccountSelectorItemViewModel? CurrentMarketplaceAccount { get; set; }
    }
}
