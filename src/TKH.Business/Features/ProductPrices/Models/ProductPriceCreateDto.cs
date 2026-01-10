using TKH.Core.Entities.Abstract;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ProductPrices.Models
{
    public class ProductPriceCreateDto : IDto
    {
        public int ProductId { get; set; }
        public ProductPriceType Type { get; set; }
        public decimal? Amount { get; set; }
    }
}
