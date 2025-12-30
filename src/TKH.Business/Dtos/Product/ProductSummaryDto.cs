using TKH.Entities.Enums;

namespace TKH.Business.Dtos.Product
{
    public record ProductSummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public string ModelCode { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
        public string Barcode { get; init; } = string.Empty;
        public string CategoryName { get; init; } = string.Empty;
        public decimal SellingPrice { get; init; }
        public decimal ListPrice { get; init; }
        public int StockQuantity { get; init; }
        public string VariantSummary { get; init; } = string.Empty;
        public bool IsOnSale { get; init; }
        public bool IsApproved { get; init; }
    }
}
