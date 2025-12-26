using System.ComponentModel;

namespace TKH.Entities.Enums
{
    public enum OrderItemStatus
    {
        [Description("Oluşturuldu")]
        Created = 1,

        [Description("Kargolandı")]
        Shipped = 20,

        [Description("Teslim Edildi")]
        Delivered = 30,

        [Description("Müşteri İptali")]
        CancelledByCustomer = 60,

        [Description("Satıcı İptali")]
        CancelledBySeller = 61,

        [Description("Sistem İptali")]
        CancelledByMarketplace = 62,

        [Description("İade Talebi Oluşturuldu")]
        ReturnRequested = 50,

        [Description("İade Kargoda")]
        ReturnShipped = 51,

        [Description("İade Teslim Alındı")]
        ReturnDeliveredToSeller = 52,

        [Description("İade Onaylandı")]
        Returned = 53,

        [Description("İade Reddedildi")]
        ReturnRejected = 54,

        [Description("Teslimat Başarısız")]
        Undelivered = 40,

        [Description("Tazmin Edildi")]
        Compensated = 80,

        [Description("Bilinmiyor")]
        Other = 99
    }
}
