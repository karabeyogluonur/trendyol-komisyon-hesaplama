namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceShipmentTransactionDto
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceOrderNumber { get; set; } = string.Empty;
        public string MarketplaceParcelId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Desi { get; set; }
    }
}
