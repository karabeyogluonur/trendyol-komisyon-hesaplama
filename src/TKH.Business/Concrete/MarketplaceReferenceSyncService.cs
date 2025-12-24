using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
{
    public class MarketplaceReferenceSyncService : IMarketplaceReferenceSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryAttribute> _categoryAttributeRepository;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public MarketplaceReferenceSyncService(
            IUnitOfWork unitOfWork,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<Category>();
            _categoryAttributeRepository = _unitOfWork.GetRepository<CategoryAttribute>();
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncCategoriesAsync(MarketplaceType marketplaceType)
        {
            IMarketplaceReferenceProvider provider = _marketplaceProviderFactory.GetProvider<IMarketplaceReferenceProvider>(marketplaceType);

            List<MarketplaceCategoryDto> incomingCategories = await provider.GetCategoryTreeAsync();

            if (incomingCategories == null || incomingCategories.Count == 0)
                return;

            List<Category> existingCategories = await _categoryRepository.GetAllAsync(x => x.MarketplaceType == marketplaceType);

            List<Category> categoriesToUpsert = new List<Category>();

            foreach (MarketplaceCategoryDto dto in incomingCategories)
            {
                Category existingCategory = existingCategories.FirstOrDefault(x => x.MarketplaceCategoryId == dto.MarketplaceCategoryId);

                if (existingCategory != null)
                {
                    existingCategory.Name = dto.Name;
                    existingCategory.ParentMarketplaceCategoryId = dto.ParentMarketplaceCategoryId;
                    existingCategory.IsLeaf = dto.IsLeaf;

                    categoriesToUpsert.Add(existingCategory);
                }
                else
                {
                    Category newCategory = _mapper.Map<Category>(dto);
                    newCategory.MarketplaceType = marketplaceType;

                    categoriesToUpsert.Add(newCategory);
                }
            }

            if (categoriesToUpsert.Count > 0)
            {
                _categoryRepository.AddOrUpdate(categoriesToUpsert);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SyncCategoryAttributesAsync(MarketplaceType marketplaceType)
        {
            IMarketplaceReferenceProvider provider = _marketplaceProviderFactory.GetProvider<IMarketplaceReferenceProvider>(marketplaceType);

            List<Category> leafCategories = await _categoryRepository.GetAllAsync(x => x.MarketplaceType == marketplaceType && x.IsLeaf);

            foreach (Category category in leafCategories)
            {
                List<MarketplaceCategoryAttributeDto> attributeDtos = await provider.GetCategoryAttributesAsync(category.MarketplaceCategoryId);

                if (attributeDtos == null || attributeDtos.Count == 0)
                    continue;

                List<CategoryAttribute> existingAttributes = await _categoryAttributeRepository.GetAllAsync(x => x.CategoryId == category.Id,
                includes: include => include.AttributeValues);

                List<CategoryAttribute> attributesToUpsert = new List<CategoryAttribute>();

                foreach (MarketplaceCategoryAttributeDto attrDto in attributeDtos)
                {
                    CategoryAttribute existingAttr = existingAttributes.FirstOrDefault(x => x.MarketplaceAttributeId == attrDto.MarketplaceAttributeId);

                    if (existingAttr != null)
                    {
                        existingAttr.Name = attrDto.Name;
                        existingAttr.IsVarianter = attrDto.IsVarianter;
                        SyncAttributeValues(existingAttr, attrDto.AttributeValues);
                        attributesToUpsert.Add(existingAttr);
                    }
                    else
                    {
                        CategoryAttribute newAttr = new CategoryAttribute
                        {
                            CategoryId = category.Id,
                            MarketplaceAttributeId = attrDto.MarketplaceAttributeId,
                            Name = attrDto.Name,
                            IsVarianter = attrDto.IsVarianter,
                            AttributeValues = attrDto.AttributeValues.Select(v => new AttributeValue
                            {
                                MarketplaceValueId = v.MarketplaceValueId,
                                Value = v.Value
                            }).ToList()
                        };
                        attributesToUpsert.Add(newAttr);
                    }
                }

                if (attributesToUpsert.Count > 0)
                {
                    _categoryAttributeRepository.AddOrUpdate(attributesToUpsert);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        private void SyncAttributeValues(CategoryAttribute attributeEntity, List<MarketplaceAttributeValueDto> incomingValues)
        {
            foreach (MarketplaceAttributeValueDto valDto in incomingValues)
            {
                AttributeValue existingVal = attributeEntity.AttributeValues
                    .FirstOrDefault(v => v.MarketplaceValueId == valDto.MarketplaceValueId);

                if (existingVal != null)
                    existingVal.Value = valDto.Value;

                else
                {
                    attributeEntity.AttributeValues.Add(new AttributeValue
                    {
                        MarketplaceValueId = valDto.MarketplaceValueId,
                        Value = valDto.Value
                    });
                }
            }
        }
    }
}
