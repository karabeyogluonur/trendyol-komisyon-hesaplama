using System.ComponentModel.DataAnnotations;

namespace TKH.Presentation.Features.Settings.Models
{
    public class TaxSettingsViewModel
    {
        [Display(Name = "Stopaj Oranı (%)")]
        public decimal WithholdingRate { get; set; }

        [Display(Name = "Kargo KDV Oranı (%)")]
        public decimal ShippingVatRate { get; set; }
    }
}
