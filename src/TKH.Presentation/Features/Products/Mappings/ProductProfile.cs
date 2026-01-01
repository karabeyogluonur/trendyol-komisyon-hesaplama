using AutoMapper;
using TKH.Business.Features.Products.Dtos;
using TKH.Presentation.Features.Products.Models;

namespace TKH.Presentation.Features.Products.Mappings
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
