using System.ComponentModel;

namespace TKH.Entities.Enums
{
    public enum ClaimReasonType
    {
        [Description("Beden Uymadı")]
        Unfit = 1,

        [Description("Kusurlu / Yanlış Ürün")]
        Defective = 2,

        [Description("Vazgeçti / Beğenmedi")]
        ChangedMind = 3,

        [Description("Teslimat Sorunu")]
        DeliveryFailure = 4,

        [Description("Teslim Alınmadı")]
        CustomerNotPickedUp = 5,

        [Description("Analiz / İnceleme")]
        // Ürün incelenmek üzere geri geliyor
        Analysis = 6,

        [Description("Diğer")]
        Other = 99
    }
}
