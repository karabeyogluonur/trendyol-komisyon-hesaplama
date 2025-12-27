using System.ComponentModel;

namespace TKH.Entities.Enums
{
    public enum ProductUnitType
    {
        [Description("Adet")]
        Piece = 1,

        [Description("Kilogram")]
        Kilogram = 2,

        [Description("Gram")]
        Gram = 3,

        [Description("Metre")]
        Meter = 4,

        [Description("Litre")]
        Liter = 5,

        [Description("Paket")]
        Packet = 6,

        [Description("Set")]
        Set = 7,

        [Description("Çift")]
        Pair = 8,

        [Description("Bilinmiyor")]
        Other = 99
    }
}
