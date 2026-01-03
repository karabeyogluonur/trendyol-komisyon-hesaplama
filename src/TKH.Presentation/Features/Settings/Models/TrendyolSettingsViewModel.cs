using System.ComponentModel.DataAnnotations;

namespace TKH.Presentation.Features.Settings.Models
{
    public class TrendyolSettingsViewModel
    {
        [Display(Name = "Trendyol Hizmet Bedeli (Kdv Dahil)")]
        public decimal ServiceFeeAmount { get; set; }

        [Display(Name = "Hizmet Bedeli KDV Oranı")]
        public decimal ServiceFeeVatRate { get; set; }

        [Display(Name = "Ürün Komisyon KDV Oranı")]
        public decimal ProductCommissionVatRate { get; set; }

        [Display(Name = "Aynı Gün Teslimat Trendyol Hizmet Bedeli (Kdv Dahil)")]
        public decimal SameDayServiceFeeAmount { get; set; }

        [Display(Name = "İhracat Hizmet Kesinti Oranı")]
        public decimal ExportServiceFeeRate { get; set; }

        [Display(Name = "İhracat Hizmet KDV Oranı")]
        public decimal ExportServiceFeeVatRate { get; set; }

        [Display(Name = "API Base URL")]
        public string BaseUrl { get; set; } = string.Empty;

        [Display(Name = "User Agent (Entegratör Kimliği)")]
        public string UserAgent { get; set; } = string.Empty;
    }
}
