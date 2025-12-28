using TKH.Core.Entities.Abstract;
using TKH.Entities.Enums;

namespace TKH.Business.Dtos.MarketplaceAccount
{
    public class MarketplaceAccountDetailsDto : IDto
    {
        public int Id { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public MarketplaceConnectionState ConnectionState { get; set; }
        public MarketplaceSyncState SyncState { get; set; }
        public string? LastErrorMessage { get; set; }
        public DateTime? LastErrorDate { get; set; }
        public DateTime? LastSyncStartTime { get; set; }
    }
}
