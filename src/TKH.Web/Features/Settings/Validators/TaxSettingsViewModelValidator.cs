using FluentValidation;
using TKH.Web.Features.Settings.Models;

namespace TKH.Web.Features.Settings.Validators
{
    public class TaxSettingsViewModelValidator : AbstractValidator<TaxSettingsViewModel>
    {
        public TaxSettingsViewModelValidator()
        {
            RuleFor(taxSetting => taxSetting.WithholdingRate)
                .NotNull().WithMessage("Stopaj oranı boş bırakılamaz.")
                .InclusiveBetween(0, 100).WithMessage("Stopaj oranı 0 ile 100 arasında olmalıdır.");

            RuleFor(taxSetting => taxSetting.ShippingVatRate)
                .NotNull().WithMessage("Kargo KDV oranı boş bırakılamaz.")
                .InclusiveBetween(0, 100).WithMessage("Kargo KDV oranı 0 ile 100 arasında olmalıdır.");
        }
    }
}
