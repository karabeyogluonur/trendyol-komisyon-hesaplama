using AutoMapper;
using TKH.Business.Features.Products.Dtos;
using TKH.Web.Features.ProductProfits.Models;

namespace TKH.Web.Features.ProductProfits.Mappings
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
