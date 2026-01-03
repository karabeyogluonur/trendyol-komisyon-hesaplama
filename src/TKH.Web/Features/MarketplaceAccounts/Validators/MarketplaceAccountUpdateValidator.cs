using FluentValidation;
using TKH.Web.Features.MarketplaceAccounts.Models;

namespace TKH.Web.Features.MarketplaceAccounts.Validators
{
    public class MarketplaceAccountUpdateValidator : AbstractValidator<MarketplaceAccountUpdateViewModel>
    {
        public MarketplaceAccountUpdateValidator()
        {
            RuleFor(x => x.StoreName).NotEmpty().WithMessage("Mağaza adı zorunludur.");
            RuleFor(x => x.MerchantId).NotEmpty().WithMessage("Merchant ID zorunludur.");
            RuleFor(x => x.ApiKey).NotEmpty().WithMessage("API Key zorunludur.");
        }
    }
}
