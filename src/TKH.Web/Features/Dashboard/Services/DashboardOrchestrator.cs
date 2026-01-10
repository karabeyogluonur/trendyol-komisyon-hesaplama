using AutoMapper;

using TKH.Business.Features.Analytics.Dtos;
using TKH.Business.Features.Analytics.Services;
using TKH.Core.Contexts;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.Dashboard.Models;

namespace TKH.Web.Features.Dashboard.Services
{
    public class DashboardOrchestrator : IDashboardOrchestrator
    {
        private readonly IProductReportService _productReportService;
        private readonly IWorkContext _workContext;
        private readonly IMapper _mapper;

        public DashboardOrchestrator(
            IProductReportService productReportService,
            IWorkContext workContext,
            IMapper mapper)
        {
            _productReportService = productReportService;
            _workContext = workContext;
            _mapper = mapper;
        }

        public async Task<IDataResult<DashboardViewModel>> PrepareDashboardViewModelAsync()
        {
            int? currentAccountId = _workContext.CurrentMarketplaceAccountId;

            if (!currentAccountId.HasValue || currentAccountId <= 0)
                return new ErrorDataResult<DashboardViewModel>("Aktif bir mağaza seçilmemiş.");

            DashboardViewModel dashboardViewModel = new DashboardViewModel();

            IDataResult<ProductCostReadinessReportDto> readinessReportResult = await _productReportService.GetProductCostReadinessReportAsync();

            if (readinessReportResult.Success && readinessReportResult.Data != null)
                dashboardViewModel.ProductCostReadiness = _mapper.Map<ProductCostReadinessWidgetViewModel>(readinessReportResult.Data);

            return new SuccessDataResult<DashboardViewModel>(dashboardViewModel);
        }
    }
}
