using AutoMapper;
using TKH.Business.Dtos.Product;
using TKH.Presentation.Models.Product;

namespace TKH.Presentation.Profiles
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
