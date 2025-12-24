using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceOrderDto
    {
        public string MarketplaceOrderNumber { get; set; } = string.Empty;
        public int MarketplaceAccountId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<MarketplaceOrderItemDto> Items { get; set; } = new();
    }

    public class MarketplaceOrderItemDto
    {
        public string Barcode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal MarketplaceDiscount { get; set; }
        public decimal SellerDiscount { get; set; }
        public OrderItemStatus Status { get; set; }
    }
}
