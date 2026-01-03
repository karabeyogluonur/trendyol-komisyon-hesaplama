namespace TKH.Web.Features.Products.Models
{
    public record ProductListItemViewModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
        public string Barcode { get; init; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public string ModelCode { get; init; } = string.Empty;
        public string CategoryName { get; init; } = string.Empty;
        public string MarketplaceName { get; init; } = string.Empty;
        public decimal SellingPrice { get; init; }
        public decimal ListPrice { get; init; }
        public int StockQuantity { get; init; }
        public string VariantSummary { get; init; } = string.Empty;
        public bool IsOnSale { get; init; }
        public bool IsApproved { get; init; }
        public bool IsLocked { get; init; }
        public string StockStatusClass => StockQuantity switch
        {
            0 => "danger",         // Kırmızı
            < 10 => "warning",     // Sarı
            _ => "success"         // Yeşil
        };
    }
}
