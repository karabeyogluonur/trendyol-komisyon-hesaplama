using TKH.Core.Entities.Abstract;

namespace TKH.Core.Common.Settings
{
    public class TaxSettings : ISettings
    {
        public decimal WithholdingRate { get; set; } = 1;
        public decimal ShippingVatRate { get; set; } = 20;
    }
}
