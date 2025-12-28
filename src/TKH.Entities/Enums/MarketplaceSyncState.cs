using System.ComponentModel.DataAnnotations;

namespace TKH.Entities.Enums
{
    public enum MarketplaceSyncState
    {
        [Display(Name = "Müsait")]
        Idle = 1,

        [Display(Name = "Sırada Bekliyor")]
        Queued = 2,

        [Display(Name = "Veri Eşitleniyor...")]
        Syncing = 3,

        [Display(Name = "Durduruluyor...")]
        Stopping = 4
    }
}
