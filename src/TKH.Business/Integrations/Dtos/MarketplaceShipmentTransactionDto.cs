namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceShipmentTransactionDto
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalOrderNumber { get; set; } = string.Empty;
        public string ExternalParcelId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Deci { get; set; }
    }
}
