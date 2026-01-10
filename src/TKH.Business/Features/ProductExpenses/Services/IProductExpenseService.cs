using TKH.Business.Features.ProductExpenses.Dtos;
using TKH.Core.Utilities.Results;

namespace TKH.Business.Features.ProductExpenses.Services
{
    public interface IProductExpenseService
    {
        Task<IResult> CreateProductExpenseAsync(ProductExpenseCreateDto productExpenseCreateDto);
        Task<IResult> CreateProductExpensesAsync(List<ProductExpenseCreateDto> productExpenseCreateDtos);




    }
}
