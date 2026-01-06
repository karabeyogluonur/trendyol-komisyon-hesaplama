using TKH.Core.Entities.Abstract;
using TKH.Entities.Enums;

namespace TKH.Business.Features.ProductExpenses.Dtos
{
    public class ProductExpenseAddDto : IDto
    {
        public int ProductId { get; set; }
        public ProductExpenseType Type { get; set; }
        public GenerationType GenerationType { get; set; }
        public decimal Amount { get; set; }
    }
}
