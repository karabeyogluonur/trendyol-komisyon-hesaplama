namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceProductDto
    {
        public string MarketplaceProductId { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
    }
}
