using AutoMapper;
using TKH.Business.Features.Products.Dtos;
using TKH.Presentation.Models.Product;

namespace TKH.Presentation.Configuration.Profiles
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
