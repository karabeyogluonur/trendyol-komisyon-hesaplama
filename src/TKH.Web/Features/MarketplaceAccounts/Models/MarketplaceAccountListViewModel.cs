using System.ComponentModel.DataAnnotations;
using TKH.Entities.Enums;

namespace TKH.Web.Features.MarketplaceAccounts.Models
{
    public class MarketplaceAccountListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Pazaryeri")]
        public MarketplaceType MarketplaceType { get; set; }

        [Display(Name = "Mağaza Adı")]
        public string StoreName { get; set; } = string.Empty;

        [Display(Name = "Satıcı ID")]
        public string MerchantId { get; set; } = string.Empty;

        [Display(Name = "Durum")]
        public bool IsActive { get; set; }

        [Display(Name = "Bağlantı Durumu")]
        public MarketplaceConnectionState ConnectionState { get; set; }

        [Display(Name = "Senkronizasyon")]
        public MarketplaceSyncState SyncState { get; set; }

        [Display(Name = "Son Hata Mesajı")]
        public string? LastErrorMessage { get; set; }

        [Display(Name = "Hata Tarihi")]
        public DateTime? LastErrorDate { get; set; }
    }
}
