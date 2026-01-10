using Microsoft.AspNetCore.Mvc;

using TKH.Core.Utilities.Results;
using TKH.Web.Features.Dashboard.Models;
using TKH.Web.Features.Dashboard.Services;

namespace TKH.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IDashboardOrchestrator _dashboardOrchestrator;

        public HomeController(IDashboardOrchestrator dashboardOrchestrator)
        {
            _dashboardOrchestrator = dashboardOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IDataResult<DashboardViewModel> prepareDashboardViewModelResult = await _dashboardOrchestrator.PrepareDashboardViewModelAsync();

            if (!prepareDashboardViewModelResult.Success)
                return View(new DashboardViewModel());

            return View(prepareDashboardViewModelResult.Data);
        }
    }
}
