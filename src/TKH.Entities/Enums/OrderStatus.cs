using System.ComponentModel;

namespace TKH.Entities.Enums
{
    public enum OrderStatus
    {

        [Description("Sipariş Oluşturuldu")]
        Created = 1,

        [Description("Onay Bekliyor")]
        Awaiting = 2,

        [Description("Hazırlanıyor")]
        Preparing = 10,

        [Description("Faturalandı")]
        Invoiced = 11,

        [Description("Kargoya Verildi")]
        Shipped = 20,

        [Description("Teslim Edildi")]
        Delivered = 30,

        [Description("Teslim Edilemedi")]
        Undelivered = 40,

        [Description("İade Edildi")]
        Returned = 50,

        [Description("İptal Edildi")]
        Cancelled = 60,

        [Description("Tedarik Edilemedi")]
        UnSupplied = 61,

        [Description("Kısmi Teslimat/İade")]
        PartiallyCompleted = 70,

        [Description("Kayıp/Hasarlı")]
        LostOrDamaged = 80,

        [Description("Bilinmiyor")]
        Unknown = 99
    }
}
