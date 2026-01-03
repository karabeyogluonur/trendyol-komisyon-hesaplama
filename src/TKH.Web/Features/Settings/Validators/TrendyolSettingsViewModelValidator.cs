using FluentValidation;
using TKH.Web.Features.Settings.Models;

namespace TKH.Web.Features.Settings.Validators
{
    public class TrendyolSettingsViewModelValidator : AbstractValidator<TrendyolSettingsViewModel>
    {
        public TrendyolSettingsViewModelValidator()
        {
            RuleFor(trendyolSetting => trendyolSetting.ServiceFeeAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Hizmet bedeli 0 veya daha büyük olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.ServiceFeeVatRate)
                .InclusiveBetween(0, 100).WithMessage("Hizmet bedeli KDV oranı 0 ile 100 arasında olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.SameDayServiceFeeAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Aynı gün işlem bedeli 0 veya daha büyük olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.ExportServiceFeeRate)
                .InclusiveBetween(0, 100).WithMessage("İhracat hizmet oranı %0 ile %100 arasında olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.ProductCommissionVatRate)
                .InclusiveBetween(0, 100).WithMessage("Komisyon KDV oranı %0 ile %100 arasında olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.ExportServiceFeeVatRate)
                .InclusiveBetween(0, 100).WithMessage("İhracat hizmet KDV oranı %0 ile %100 arasında olmalıdır.");

            RuleFor(trendyolSetting => trendyolSetting.BaseUrl)
                .NotEmpty().WithMessage("API Base URL alanı boş bırakılamaz.")
                .Must(BeAValidUrl).WithMessage("Lütfen geçerli bir URL formatı giriniz (örn: https://api.trendyol.com).");

            RuleFor(trendyolSetting => trendyolSetting.UserAgent)
                .NotEmpty().WithMessage("User Agent bilgisi boş bırakılamaz.");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
