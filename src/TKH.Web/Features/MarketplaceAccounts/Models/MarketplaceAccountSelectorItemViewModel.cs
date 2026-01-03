using TKH.Entities.Enums;

namespace TKH.Web.Features.MarketplaceAccounts.Models
{
    public class MarketplaceAccountSelectorItemViewModel
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public MarketplaceType MarketplaceType { get; set; }
        public string MerchantId { get; set; } = string.Empty;
        public MarketplaceConnectionState ConnectionState { get; set; }
        public MarketplaceSyncState SyncState { get; set; }
    }
}
