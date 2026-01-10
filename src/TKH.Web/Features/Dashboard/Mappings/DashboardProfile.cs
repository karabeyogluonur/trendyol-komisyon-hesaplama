using AutoMapper;

using TKH.Business.Features.Analytics.Dtos;
using TKH.Web.Features.Dashboard.Models;

namespace TKH.Web.Features.Dashboard.Mappings
{
    public class DashboardProfile : Profile
    {
        public DashboardProfile()
        {
            CreateMap<ProductCostReadinessReportDto, ProductCostReadinessWidgetViewModel>();
        }
    }
}
