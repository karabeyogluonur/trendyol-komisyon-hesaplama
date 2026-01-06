using TKH.Business.Features.ProductPrices.Models;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.ProductPrices.Services
{
    public interface IProductPriceService
    {
        Task<IResult> AddAsync(ProductPriceAddDto productPriceAddDto);
        Task<IResult> AddRangeAsync(List<ProductPriceAddDto> productPriceAddDtos);
    }
}
