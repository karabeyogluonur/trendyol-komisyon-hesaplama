using System;

namespace TKH.Web.Features.ProductProfits.Models
{
    public class ProductProfitItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal VatRate { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ServiceFee { get; set; }
    }
}
