using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class Product : BaseEntity, IEntity, IHasMarketplaceAccount
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ExternalProductCode { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Deci { get; set; }
        public decimal VatRate { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnitType UnitType { get; set; } = ProductUnitType.Piece;
        public bool IsOnSale { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int MarketplaceAccountId { get; set; }
        public int? CategoryId { get; set; }
        public virtual MarketplaceAccount MarketplaceAccount { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        public virtual ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public virtual ICollection<ProductExpense> Expenses { get; set; } = new List<ProductExpense>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
