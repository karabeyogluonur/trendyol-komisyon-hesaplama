using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceShipmentSyncResultDto
    {
        public string SourceTransactionId { get; set; } = string.Empty;
        public ShipmentTransactionSyncStatus ResultStatus { get; set; }
        public List<MarketplaceShipmentTransactionDto> Shipments { get; set; } = new();
    }
}
