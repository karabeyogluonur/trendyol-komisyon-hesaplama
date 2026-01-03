using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.Categories.Services
{
    public class CategorySyncService : ICategorySyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public CategorySyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncCategoriesAsync(MarketplaceType marketplaceType)
        {
            IMarketplaceCategoryProvider marketplaceCategoryProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceCategoryProvider>(marketplaceType);
            List<MarketplaceCategoryDto> incomingMarketplaceCategoryDtos = await marketplaceCategoryProvider.GetCategoryTreeAsync();

            if (incomingMarketplaceCategoryDtos is null || incomingMarketplaceCategoryDtos.Count is 0) return;

            List<MarketplaceCategoryDto> distinctIncomingMarketplaceCategoryDtos = incomingMarketplaceCategoryDtos
                .DistinctBy(marketplaceCategoryDto => marketplaceCategoryDto.ExternalId)
                .ToList();

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Category> scopedCategoryRepository = scopedUnitOfWork.GetRepository<Category>();

                IList<Category> existingCategoryEntities = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => category.MarketplaceType == marketplaceType,
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                Dictionary<string, Category> existingCategoryEntityMap = existingCategoryEntities.ToDictionary(categoryEntity => categoryEntity.ExternalId, categoryEntity => categoryEntity);

                List<Category> newCategoryEntitiesToAdd = new List<Category>();

                foreach (MarketplaceCategoryDto incomingMarketplaceCategoryDto in distinctIncomingMarketplaceCategoryDtos)
                {
                    bool isCategoryExisting = existingCategoryEntityMap.TryGetValue(incomingMarketplaceCategoryDto.ExternalId, out Category? existingCategoryEntity);

                    if (isCategoryExisting && existingCategoryEntity is not null)
                    {
                        int preservedCategoryId = existingCategoryEntity.Id;
                        _mapper.Map(incomingMarketplaceCategoryDto, existingCategoryEntity);
                        existingCategoryEntity.Id = preservedCategoryId;
                    }
                    else
                    {
                        Category newCategoryEntity = _mapper.Map<Category>(incomingMarketplaceCategoryDto);
                        newCategoryEntity.MarketplaceType = marketplaceType;
                        newCategoryEntitiesToAdd.Add(newCategoryEntity);
                    }
                }

                if (newCategoryEntitiesToAdd.Count > 0)
                    await scopedCategoryRepository.InsertAsync(newCategoryEntitiesToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task SyncCategoryAttributesAsync(MarketplaceType marketplaceType)
        {
            IMarketplaceCategoryProvider marketplaceCategoryProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceCategoryProvider>(marketplaceType);
            IList<Category> leafCategoryEntities;

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork readOnlyUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Category> readOnlyCategoryRepository = readOnlyUnitOfWork.GetRepository<Category>();

                leafCategoryEntities = await readOnlyCategoryRepository.GetAllAsync(
                    predicate: category => category.MarketplaceType == marketplaceType && category.IsLeaf,
                    disableTracking: true,
                    ignoreQueryFilters: true
                );
            }

            IEnumerable<Category[]> categoryEntityBatches = leafCategoryEntities.Chunk(ApplicationDefaults.ReferenceBatchSize);

            using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(ApplicationDefaults.MarketplaceSyncParallelism);

            foreach (Category[] currentCategoryEntityBatch in categoryEntityBatches)
            {
                var attributeDownloadTasks = currentCategoryEntityBatch.Select(async categoryEntity =>
                {
                    await semaphoreSlim.WaitAsync();
                    try
                    {
                        List<MarketplaceCategoryAttributeDto> downloadedAttributeDtos = await marketplaceCategoryProvider.GetCategoryAttributesAsync(categoryEntity.ExternalId);
                        return new { CategoryEntity = categoryEntity, AttributeDtos = downloadedAttributeDtos };
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });

                var attributeDownloadResults = await Task.WhenAll(attributeDownloadTasks);

                var validAttributeDownloadResults = attributeDownloadResults
                    .Where(downloadResult => downloadResult.AttributeDtos is not null && downloadResult.AttributeDtos.Count > 0)
                    .ToList();

                if (validAttributeDownloadResults.Count == 0) continue;

                await ProcessCategoryAttributeBatchAsync(validAttributeDownloadResults);
            }
        }

        private async Task ProcessCategoryAttributeBatchAsync(IEnumerable<dynamic> attributeDownloadResults)
        {
            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<CategoryAttribute> scopedCategoryAttributeRepository = scopedUnitOfWork.GetRepository<CategoryAttribute>();

                List<int> categoryIdsInCurrentBatch = attributeDownloadResults.Select(downloadResult => (int)downloadResult.CategoryEntity.Id).ToList();

                IList<CategoryAttribute> existingCategoryAttributeEntities = await scopedCategoryAttributeRepository.GetAllAsync(
                    predicate: categoryAttribute => categoryIdsInCurrentBatch.Contains(categoryAttribute.CategoryId),
                    include: source => source.Include(categoryAttribute => categoryAttribute.Values),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                Dictionary<string, CategoryAttribute> existingCategoryAttributeEntityMap = existingCategoryAttributeEntities
                    .ToDictionary(categoryAttributeEntity => $"{categoryAttributeEntity.CategoryId}_{categoryAttributeEntity.ExternalId}", categoryAttributeEntity => categoryAttributeEntity);

                List<CategoryAttribute> newCategoryAttributeEntitiesToAdd = new List<CategoryAttribute>();

                foreach (var attributeDownloadResult in attributeDownloadResults)
                {
                    Category currentCategoryEntity = attributeDownloadResult.CategoryEntity;
                    List<MarketplaceCategoryAttributeDto> incomingAttributeDtos = attributeDownloadResult.AttributeDtos;

                    var distinctIncomingAttributeDtos = incomingAttributeDtos
                        .DistinctBy(attributeDto => attributeDto.ExternalId)
                        .ToList();

                    foreach (MarketplaceCategoryAttributeDto incomingAttributeDto in distinctIncomingAttributeDtos)
                    {
                        string attributeLookupKey = $"{currentCategoryEntity.Id}_{incomingAttributeDto.ExternalId}";

                        bool isAttributeExisting = existingCategoryAttributeEntityMap.TryGetValue(attributeLookupKey, out CategoryAttribute? existingCategoryAttributeEntity);

                        if (isAttributeExisting && existingCategoryAttributeEntity is not null)
                        {
                            int preservedCategoryAttributeId = existingCategoryAttributeEntity.Id;

                            _mapper.Map(incomingAttributeDto, existingCategoryAttributeEntity);

                            existingCategoryAttributeEntity.Id = preservedCategoryAttributeId;
                            existingCategoryAttributeEntity.CategoryId = currentCategoryEntity.Id;

                            SyncAttributeValuesSafe(existingCategoryAttributeEntity, incomingAttributeDto.Values);
                        }
                        else
                        {
                            CategoryAttribute newCategoryAttributeEntity = _mapper.Map<CategoryAttribute>(incomingAttributeDto);

                            newCategoryAttributeEntity.Id = 0;
                            newCategoryAttributeEntity.CategoryId = currentCategoryEntity.Id;

                            SyncAttributeValuesSafe(newCategoryAttributeEntity, incomingAttributeDto.Values);

                            newCategoryAttributeEntitiesToAdd.Add(newCategoryAttributeEntity);
                        }
                    }
                }

                if (newCategoryAttributeEntitiesToAdd.Count > 0)
                    await scopedCategoryAttributeRepository.InsertAsync(newCategoryAttributeEntitiesToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void SyncAttributeValuesSafe(CategoryAttribute parentCategoryAttributeEntity, List<MarketplaceAttributeValueDto> incomingAttributeValueDtos)
        {
            if (incomingAttributeValueDtos is null || incomingAttributeValueDtos.Count == 0) return;

            if (parentCategoryAttributeEntity.Values == null)
                parentCategoryAttributeEntity.Values = new List<AttributeValue>();

            Dictionary<string, AttributeValue> existingAttributeValueEntityMap = parentCategoryAttributeEntity.Values
                .ToDictionary(attributeValueEntity => attributeValueEntity.ExternalId, attributeValueEntity => attributeValueEntity);

            List<MarketplaceAttributeValueDto> distinctIncomingAttributeValueDtos = incomingAttributeValueDtos
                .DistinctBy(attributeValueDto => attributeValueDto.ExternalId)
                .ToList();

            foreach (MarketplaceAttributeValueDto incomingAttributeValueDto in distinctIncomingAttributeValueDtos)
            {
                if (existingAttributeValueEntityMap.TryGetValue(incomingAttributeValueDto.ExternalId, out AttributeValue? existingAttributeValueEntity))
                {
                    existingAttributeValueEntity.Value = incomingAttributeValueDto.Value;
                }
                else
                {
                    AttributeValue newAttributeValueEntity = new AttributeValue
                    {
                        ExternalId = incomingAttributeValueDto.ExternalId,
                        Value = incomingAttributeValueDto.Value,
                        CategoryAttributeId = parentCategoryAttributeEntity.Id
                    };

                    parentCategoryAttributeEntity.Values.Add(newAttributeValueEntity);
                }
            }
        }
    }
}
