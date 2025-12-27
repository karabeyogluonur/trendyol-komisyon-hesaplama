using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Product : BaseEntity, IEntity
    {
        public string MarketplaceProductId { get; set; } = string.Empty;
        public string MarketplaceProductUrl { get; set; } = string.Empty;
        public string MarketplaceProductCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double CargoDeci { get; set; }
        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnitType UnitType { get; set; } = ProductUnitType.Piece;

        public bool IsOnSale { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public int MarketplaceAccountId { get; set; }
        public MarketplaceAccount? MarketplaceAccount { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
        public ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
        public ICollection<ProductExpense> ProductExpenses { get; set; } = new List<ProductExpense>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
