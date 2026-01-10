namespace TKH.Web.Features.Products.Models
{
    public class ProductCostListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public decimal? PurchasePrice { get; set; }
        public decimal? ManualCommissionRate { get; set; }
        public decimal? AutomatedCommissionRate { get; set; }
        public decimal? ManualShippingCost { get; set; }
        public decimal? AutomatedShippingCost { get; set; }
    }
}
