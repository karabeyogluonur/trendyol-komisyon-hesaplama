using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceProductDto
    {
        public int MarketplaceAccountId { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public string ExternalProductCode { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Deci { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnitType UnitType { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
        public int ExternalCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public List<MarketplaceProductAttributeDto> Attributes { get; set; } = new();
        public List<MarketplaceProductPriceDto> Prices { get; set; } = new();
        public List<MarketplaceProductExpenseDto> Expenses { get; set; } = new();
    }

    public class MarketplaceProductAttributeDto
    {
        public string ExternalAttributeId { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string? ExternalValueId { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public class MarketplaceProductPriceDto
    {
        public ProductPriceType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsVatIncluded { get; set; } = true;
    }

    public class MarketplaceProductExpenseDto
    {
        public ProductExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal VatRate { get; set; }
        public bool IsVatIncluded { get; set; }
    }
}
