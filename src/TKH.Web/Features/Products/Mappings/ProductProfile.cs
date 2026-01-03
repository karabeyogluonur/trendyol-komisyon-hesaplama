using AutoMapper;
using TKH.Business.Features.Products.Dtos;
using TKH.Web.Features.Products.Models;

namespace TKH.Web.Features.Products.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductSummaryDto, ProductListItemViewModel>();
            CreateMap<ProductListFilterViewModel, ProductListFilterDto>();
        }
    }
}
