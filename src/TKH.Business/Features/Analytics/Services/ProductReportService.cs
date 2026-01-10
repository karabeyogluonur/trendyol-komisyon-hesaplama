using Microsoft.Extensions.Logging;

using TKH.Business.Features.Analytics.Dtos;
using TKH.Business.Features.Analytics.Services;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.Products.Reports
{
    public class ProductReportService : IProductReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;

        public ProductReportService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
        }

        public async Task<IDataResult<ProductCostReadinessReportDto>> GetProductCostReadinessReportAsync()
        {
            int missingPurchasePriceCount = await _productRepository.CountAsync(
                predicate: product => !product.Prices.Any(price =>
                    price.Type == ProductPriceType.PurchasePrice && price.EndDate == null)
            );

            int missingShippingCostCount = await _productRepository.CountAsync(
                predicate: product => !product.Expenses.Any(expense =>
                    expense.Type == ProductExpenseType.ShippingCost && expense.EndDate == null)
            );

            int missingComissionCount = await _productRepository.CountAsync(
                predicate: product => !product.Expenses.Any(expense =>
                    expense.Type == ProductExpenseType.CommissionRate && expense.EndDate == null)
            );

            int readyForAnalysisCount = await _productRepository.CountAsync(
                predicate: product =>
                    product.Prices.Any(price => price.Type == ProductPriceType.PurchasePrice && price.EndDate == null) &&
                    product.Expenses.Any(expense => expense.Type == ProductExpenseType.ShippingCost && expense.EndDate == null) &&
                    product.Expenses.Any(expense => expense.Type == ProductExpenseType.CommissionRate && expense.EndDate == null)
            );

            int totalProductCount = await _productRepository.CountAsync();

            var reportDto = new ProductCostReadinessReportDto
            {
                MissingPurchasePriceCount = missingPurchasePriceCount,
                MissingShippingCostCount = missingShippingCostCount,
                MissingCommissionCount = missingComissionCount,
                ReadyForAnalysisCount = readyForAnalysisCount,
                TotalProductCount = totalProductCount
            };

            return new SuccessDataResult<ProductCostReadinessReportDto>(reportDto, "Maliyet hazırlık raporu güncel verilere göre oluşturuldu.");
        }
    }
}
