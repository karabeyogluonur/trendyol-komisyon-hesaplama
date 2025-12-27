namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceCategoryAttributeDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsVariant { get; set; }
        public List<MarketplaceAttributeValueDto> Values { get; set; } = new();
    }
}
