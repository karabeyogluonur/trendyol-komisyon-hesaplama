namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceCategoryAttributeDto
    {
        public string MarketplaceAttributeId { get; set; }
        public string Name { get; set; }
        public bool IsVarianter { get; set; }
        public List<MarketplaceAttributeValueDto> AttributeValues { get; set; }
    }
}
