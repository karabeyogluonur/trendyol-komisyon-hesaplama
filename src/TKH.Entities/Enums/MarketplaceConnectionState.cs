using System.ComponentModel.DataAnnotations;

namespace TKH.Entities.Enums
{
    public enum MarketplaceConnectionState
    {
        [Display(Name = "Tanımsız")]
        None = 0,

        [Display(Name = "Bağlantı Başarılı")]
        Connected = 1,

        [Display(Name = "Kurulum Bekliyor")]
        Initializing = 2,

        [Display(Name = "Yetki Hatası")]
        AuthError = 3,

        [Display(Name = "Sistem Hatası")]
        SystemError = 4
    }
}
