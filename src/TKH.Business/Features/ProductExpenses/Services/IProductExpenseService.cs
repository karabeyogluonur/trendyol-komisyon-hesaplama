using TKH.Business.Features.ProductExpenses.Dtos;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public interface IProductExpenseService
    {
        Task<IResult> AddAsync(ProductExpenseAddDto productExpenseAddDto);
        Task<IResult> AddRangeAsync(List<ProductExpenseAddDto> productExpenseAddDtos);
    }
}
