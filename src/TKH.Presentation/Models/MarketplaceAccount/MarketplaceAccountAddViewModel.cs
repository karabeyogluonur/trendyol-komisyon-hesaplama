using System.ComponentModel.DataAnnotations;
using TKH.Entities.Enums;

namespace TKH.Presentation.Models.MarketplaceAccount
{
    public class MarketplaceAccountAddViewModel
    {
        [Display(Name = "Pazaryeri")]
        public MarketplaceType MarketplaceType { get; set; }

        [Display(Name = "Mağaza Adı")]
        public string StoreName { get; set; } = string.Empty;

        [Display(Name = "API Anahtarı (Api Key)")]
        public string ApiKey { get; set; } = string.Empty;

        [Display(Name = "Gizli Anahtar (Api Secret Key)")]
        public string ApiSecretKey { get; set; } = string.Empty;

        [Display(Name = "Satıcı ID (Merchant / Supplier ID)")]
        public string MerchantId { get; set; } = string.Empty;
    }
}
