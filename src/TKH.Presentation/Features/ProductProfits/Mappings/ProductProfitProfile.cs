using AutoMapper;
using TKH.Business.Features.Products.Dtos;
using TKH.Presentation.Features.ProductProfits.Models;

namespace TKH.Presentation.Features.ProductProfits.Mappings
{
    public class ProductProfitProfile : Profile
    {
        public ProductProfitProfile()
        {
            CreateMap<ProductProfitSummaryDto, ProductProfitItemViewModel>();
            CreateMap<ProductProfitListFilterViewModel, ProductProfitListFilterDto>();
        }
    }
}
