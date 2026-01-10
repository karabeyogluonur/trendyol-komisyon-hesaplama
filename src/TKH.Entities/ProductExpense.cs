using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class ProductExpense : BaseEntity, IEntity
    {
        #region Properties

        public int ProductId { get; private set; }
        public virtual Product Product { get; private set; }

        public ProductExpenseType Type { get; private set; }
        public GenerationType GenerationType { get; private set; }

        public decimal Amount { get; private set; }
        public decimal VatRate { get; private set; }
        public bool IsVatIncluded { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        #endregion

        #region Ctor

        protected ProductExpense()
        {
        }

        #endregion

        #region Factory

        public static ProductExpense Create(
            int productId,
            ProductExpenseType productExpenseType,
            decimal amount,
            GenerationType generationType,
            decimal vatRate,
            bool isVatIncluded)
        {
            return new ProductExpense
            {
                ProductId = productId,
                Type = productExpenseType,
                Amount = amount,
                GenerationType = generationType,
                VatRate = vatRate,
                IsVatIncluded = isVatIncluded,
                StartDate = DateTime.UtcNow,
                EndDate = null
            };
        }

        #endregion

        #region Behavior

        public void MarkAsEnded(DateTime endDate)
        {
            if (EndDate.HasValue)
                return;

            EndDate = endDate;
        }

        public bool IsDifferent(decimal amount, decimal vatRate, bool isVatIncluded)
        {
            return Amount != amount || VatRate != vatRate || IsVatIncluded != isVatIncluded;
        }

        #endregion
    }
}
