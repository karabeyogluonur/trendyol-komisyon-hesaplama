using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;

namespace TKH.Entities
{
    public class Product : BaseEntity, IEntity
    {
        public int MarketplaceAccountId { get; set; }
        public string MarketplaceProductId { get; set; } = string.Empty;
        public string? Barcode { get; set; } = string.Empty;
        public string? StockCode { get; set; } = string.Empty;
        public string? ModelCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal VatRate { get; set; }
        public decimal CommissionRate { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public MarketplaceAccount MarketplaceAccount { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
