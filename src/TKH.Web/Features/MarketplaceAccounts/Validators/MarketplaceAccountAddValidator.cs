using FluentValidation;
using TKH.Web.Features.MarketplaceAccounts.Models;

namespace TKH.Web.Features.MarketplaceAccounts.Validators
{
    public class MarketplaceAccountAddValidator : AbstractValidator<MarketplaceAccountAddViewModel>
    {
        public MarketplaceAccountAddValidator()
        {
            RuleFor(x => x.MarketplaceType)
                .IsInEnum().WithMessage("Geçerli bir pazar yeri seçmelisiniz.")
                .NotEmpty().WithMessage("Lütfen bir pazar yeri seçiniz.");

            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("Mağaza adı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Mağaza adı en az 3 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Mağaza adı 50 karakteri geçemez.");

            RuleFor(x => x.MerchantId)
                .NotEmpty().WithMessage("Satıcı ID (Merchant Id) alanı zorunludur.");

            RuleFor(x => x.ApiKey)
                .NotEmpty().WithMessage("API Key alanı zorunludur.");

            RuleFor(x => x.ApiSecretKey)
                .NotEmpty().WithMessage("API Secret Key alanı zorunludur.");

            When(x => x.MarketplaceType == Entities.Enums.MarketplaceType.Trendyol, () =>
            {
                RuleFor(x => x.MerchantId).Matches(@"^\d+$").WithMessage("Trendyol ID sadece rakamlardan oluşur.");
            });
        }
    }
}
