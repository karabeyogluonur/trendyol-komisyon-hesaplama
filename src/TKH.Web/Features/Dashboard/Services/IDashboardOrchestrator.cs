using TKH.Core.Utilities.Results;
using TKH.Web.Features.Dashboard.Models;

namespace TKH.Web.Features.Dashboard.Services
{
    public interface IDashboardOrchestrator
    {
        Task<IDataResult<DashboardViewModel>> PrepareDashboardViewModelAsync();
    }
}
