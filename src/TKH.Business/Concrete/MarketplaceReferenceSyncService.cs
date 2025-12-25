using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using TKH.Business.Abstract;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
{
    public class MarketplaceReferenceSyncService : IMarketplaceReferenceSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public MarketplaceReferenceSyncService(
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
            IMarketplaceReferenceProvider marketplaceReferenceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceReferenceProvider>(marketplaceType);
            List<MarketplaceCategoryDto> incomingMarketplaceCategoryDtos = await marketplaceReferenceProvider.GetCategoryTreeAsync();

            if (incomingMarketplaceCategoryDtos is null || incomingMarketplaceCategoryDtos.Count == 0) return;

            List<MarketplaceCategoryDto> distinctIncomingMarketplaceCategoryDtos = incomingMarketplaceCategoryDtos
                .DistinctBy(marketplaceCategoryDto => marketplaceCategoryDto.MarketplaceCategoryId)
                .ToList();

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Category> scopedCategoryRepository = scopedUnitOfWork.GetRepository<Category>();

                IList<Category> existingCategoryEntities = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => category.MarketplaceType == marketplaceType,
                    disableTracking: false
                );

                Dictionary<string, Category> existingCategoryEntityMap = existingCategoryEntities
                    .ToDictionary(categoryEntity => categoryEntity.MarketplaceCategoryId, categoryEntity => categoryEntity);

                List<Category> newCategoryEntitiesToAdd = new List<Category>();

                foreach (MarketplaceCategoryDto incomingMarketplaceCategoryDto in distinctIncomingMarketplaceCategoryDtos)
                {
                    bool isCategoryExisting = existingCategoryEntityMap.TryGetValue(incomingMarketplaceCategoryDto.MarketplaceCategoryId, out Category? existingCategoryEntity);

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
            IMarketplaceReferenceProvider marketplaceReferenceProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceReferenceProvider>(marketplaceType);
            IList<Category> leafCategoryEntities;

            using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork readOnlyUnitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Category> readOnlyCategoryRepository = readOnlyUnitOfWork.GetRepository<Category>();

                leafCategoryEntities = await readOnlyCategoryRepository.GetAllAsync(
                    predicate: category => category.MarketplaceType == marketplaceType && category.IsLeaf,
                    disableTracking: true
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
                        List<MarketplaceCategoryAttributeDto> downloadedAttributeDtos = await marketplaceReferenceProvider.GetCategoryAttributesAsync(categoryEntity.MarketplaceCategoryId);
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
                    include: source => source.Include(categoryAttribute => categoryAttribute.AttributeValues),
                    disableTracking: false
                );

                Dictionary<string, CategoryAttribute> existingCategoryAttributeEntityMap = existingCategoryAttributeEntities
                    .ToDictionary(categoryAttributeEntity => $"{categoryAttributeEntity.CategoryId}_{categoryAttributeEntity.MarketplaceAttributeId}", categoryAttributeEntity => categoryAttributeEntity);

                List<CategoryAttribute> newCategoryAttributeEntitiesToAdd = new List<CategoryAttribute>();

                foreach (var attributeDownloadResult in attributeDownloadResults)
                {
                    Category currentCategoryEntity = attributeDownloadResult.CategoryEntity;
                    List<MarketplaceCategoryAttributeDto> incomingAttributeDtos = attributeDownloadResult.AttributeDtos;

                    var distinctIncomingAttributeDtos = incomingAttributeDtos
                        .DistinctBy(attributeDto => attributeDto.MarketplaceAttributeId)
                        .ToList();

                    foreach (MarketplaceCategoryAttributeDto incomingAttributeDto in distinctIncomingAttributeDtos)
                    {
                        string attributeLookupKey = $"{currentCategoryEntity.Id}_{incomingAttributeDto.MarketplaceAttributeId}";

                        bool isAttributeExisting = existingCategoryAttributeEntityMap.TryGetValue(attributeLookupKey, out CategoryAttribute? existingCategoryAttributeEntity);

                        if (isAttributeExisting && existingCategoryAttributeEntity is not null)
                        {
                            int preservedCategoryAttributeId = existingCategoryAttributeEntity.Id;

                            _mapper.Map(incomingAttributeDto, existingCategoryAttributeEntity);

                            existingCategoryAttributeEntity.Id = preservedCategoryAttributeId;
                            existingCategoryAttributeEntity.CategoryId = currentCategoryEntity.Id;

                            SyncAttributeValuesSafe(existingCategoryAttributeEntity, incomingAttributeDto.AttributeValues);
                        }
                        else
                        {
                            CategoryAttribute newCategoryAttributeEntity = _mapper.Map<CategoryAttribute>(incomingAttributeDto);

                            newCategoryAttributeEntity.Id = 0;
                            newCategoryAttributeEntity.CategoryId = currentCategoryEntity.Id;

                            SyncAttributeValuesSafe(newCategoryAttributeEntity, incomingAttributeDto.AttributeValues);

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

            if (parentCategoryAttributeEntity.AttributeValues == null)
                parentCategoryAttributeEntity.AttributeValues = new List<AttributeValue>();

            Dictionary<string, AttributeValue> existingAttributeValueEntityMap = parentCategoryAttributeEntity.AttributeValues
                .ToDictionary(attributeValueEntity => attributeValueEntity.MarketplaceValueId, attributeValueEntity => attributeValueEntity);

            List<MarketplaceAttributeValueDto> distinctIncomingAttributeValueDtos = incomingAttributeValueDtos
                .DistinctBy(attributeValueDto => attributeValueDto.MarketplaceValueId)
                .ToList();

            foreach (MarketplaceAttributeValueDto incomingAttributeValueDto in distinctIncomingAttributeValueDtos)
            {
                if (existingAttributeValueEntityMap.TryGetValue(incomingAttributeValueDto.MarketplaceValueId, out AttributeValue? existingAttributeValueEntity))
                {
                    existingAttributeValueEntity.Value = incomingAttributeValueDto.Value;
                }
                else
                {
                    AttributeValue newAttributeValueEntity = new AttributeValue
                    {
                        MarketplaceValueId = incomingAttributeValueDto.MarketplaceValueId,
                        Value = incomingAttributeValueDto.Value,
                        CategoryAttributeId = parentCategoryAttributeEntity.Id
                    };

                    parentCategoryAttributeEntity.AttributeValues.Add(newAttributeValueEntity);
                }
            }
        }
    }
}
