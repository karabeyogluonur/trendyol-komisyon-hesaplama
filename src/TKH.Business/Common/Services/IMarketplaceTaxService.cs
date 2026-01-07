using TKH.Entities.Enums;

namespace TKH.Business.Common.Services
{
    public interface IMarketplaceTaxService
    {
        decimal GetVatRateByExpenseType(MarketplaceType marketplaceType, ProductExpenseType expenseType);
    }
}
