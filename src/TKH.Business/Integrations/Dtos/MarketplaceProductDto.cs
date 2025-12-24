namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceProductDto
    {
        public int MarketplaceAccountId { get; set; }

        public string MarketplaceProductId { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
        public string MarketplaceCategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<MarketplaceProductAttributeDto> Attributes { get; set; } = new();
    }

    public class MarketplaceProductAttributeDto
    {
        public string MarketplaceAttributeId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string? MarketplaceValueId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
