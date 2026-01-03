namespace TKH.Business.Integrations.Marketplaces.Dtos
{
    public class MarketplaceDefaultsDto
    {
        public decimal ServiceFee { get; set; }
        public decimal ServiceFeeVatRate { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
