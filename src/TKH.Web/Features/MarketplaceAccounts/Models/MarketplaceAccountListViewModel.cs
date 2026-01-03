using TKH.Entities.Enums;

namespace TKH.Web.Features.MarketplaceAccounts.Models
{
    public class MarketplaceAccountListViewModel
    {
        public int Id { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public MarketplaceConnectionState ConnectionState { get; set; }
        public MarketplaceSyncState SyncState { get; set; }
        public string? LastErrorMessage { get; set; }
        public DateTime? LastErrorDate { get; set; }
    }
}
