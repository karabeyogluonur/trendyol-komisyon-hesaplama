namespace TKH.Web.Features.Products.Models
{
    public class ProductCostBatchViewModel
    {
        public int Id { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ManualCommissionRate { get; set; }
        public decimal? ManualShippingCost { get; set; }
    }
}
