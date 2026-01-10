using TKH.Business.Features.ProductPrices.Models;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.ProductPrices.Services
{
    public interface IProductPriceService
    {
        Task<IResult> CreateProductPriceAsync(ProductPriceCreateDto productPriceCreateDto);
        Task<IResult> CreateProductPricesAsync(List<ProductPriceCreateDto> productPriceCreateDtos);
    }
}
