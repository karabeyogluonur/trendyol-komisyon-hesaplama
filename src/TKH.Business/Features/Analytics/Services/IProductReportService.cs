using TKH.Business.Features.Analytics.Dtos;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.Analytics.Services
{
    public interface IProductReportService
    {
        Task<IDataResult<ProductCostReadinessReportDto>> GetProductCostReadinessReportAsync();
    }
}
