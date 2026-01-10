using TKH.Core.Entities.Abstract;

namespace TKH.Business.Features.Products.Dtos
{
    public class ProductProfitSummaryDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ExternalUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? ManualCommissionRate { get; set; }
        public decimal? AutomatedCommissionRate { get; set; }
        public decimal VatRate { get; set; }
        public decimal? ManualShippingCost { get; set; }
        public decimal? AutomatedShippingCost { get; set; }
        public decimal ServiceFee { get; set; }
    }
}
