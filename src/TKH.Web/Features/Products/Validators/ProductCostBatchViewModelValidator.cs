using FluentValidation;
using TKH.Web.Features.Products.Models;

namespace TKH.Web.Features.Products.Validators
{
    public class ProductCostBatchViewModelValidator : AbstractValidator<ProductCostBatchViewModel>
    {
        public ProductCostBatchViewModelValidator()
        {
            RuleFor(product => product.Id).GreaterThan(0).WithMessage("Geçerli bir ürün seçilmelidir.");
            RuleFor(product => product.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage("Alış fiyatı negatif olamaz.");
            RuleFor(product => product.ManualShippingCost).GreaterThanOrEqualTo(0).WithMessage("Kargo bedeli negatif olamaz.");
            RuleFor(product => product.ManualCommissionRate).GreaterThanOrEqualTo(0).WithMessage("Komisyon oranı negatif olamaz.").LessThanOrEqualTo(100).WithMessage("Komisyon oranı %100’den büyük olamaz.");
        }
    }

    public class ProductCostBatchListValidator : AbstractValidator<List<ProductCostBatchViewModel>>
    {
        public ProductCostBatchListValidator()
        {
            RuleForEach(productCost => productCost).SetValidator(new ProductCostBatchViewModelValidator());
        }
    }
}
