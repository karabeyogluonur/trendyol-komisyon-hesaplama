using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TKH.Business.Features.Categories.Dtos;
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
        private readonly ILogger<CategorySyncService> _logger;

        public CategorySyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            ILogger<CategorySyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _logger = logger;
        }

        public async Task SyncCategoriesAsync(MarketplaceType marketplaceType)
        {
            _logger.LogInformation("Starting category sync for MarketplaceType: {MarketplaceType}", marketplaceType);

            IMarketplaceCategoryProvider marketplaceCategoryProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceCategoryProvider>(marketplaceType);
            List<MarketplaceCategoryDto> incomingMarketplaceCategoryDtos = await marketplaceCategoryProvider.GetCategoryTreeAsync();

            if (incomingMarketplaceCategoryDtos is null || incomingMarketplaceCategoryDtos.Count == 0)
                return;

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

                Dictionary<string, Category> existingCategoryEntityMapDictionary = existingCategoryEntities.ToDictionary(categoryEntity => categoryEntity.ExternalId, categoryEntity => categoryEntity);

                List<Category> newCategoryEntitiesToAddList = new List<Category>();

                foreach (MarketplaceCategoryDto incomingMarketplaceCategoryDto in distinctIncomingMarketplaceCategoryDtos)
                {
                    bool isCategoryExisting = existingCategoryEntityMapDictionary.TryGetValue(incomingMarketplaceCategoryDto.ExternalId, out Category? existingCategoryEntity);

                    if (isCategoryExisting && existingCategoryEntity is not null)
                    {
                        existingCategoryEntity.UpdateDetails(
                            incomingMarketplaceCategoryDto.ParentExternalId,
                            incomingMarketplaceCategoryDto.Name,
                            incomingMarketplaceCategoryDto.IsLeaf
                        );
                    }
                    else
                    {
                        Category newCategoryEntity = Category.Create(
                            marketplaceType,
                            incomingMarketplaceCategoryDto.ExternalId,
                            incomingMarketplaceCategoryDto.ParentExternalId,
                            incomingMarketplaceCategoryDto.Name,
                            incomingMarketplaceCategoryDto.IsLeaf
                        );

                        newCategoryEntitiesToAddList.Add(newCategoryEntity);
                    }
                }

                if (newCategoryEntitiesToAddList.Count > 0)
                {
                    await scopedCategoryRepository.InsertAsync(newCategoryEntitiesToAddList);
                }

                await scopedUnitOfWork.SaveChangesAsync();
                _logger.LogInformation("Category sync completed.");
            }
        }

        public async Task SyncCategoryAttributesAsync(MarketplaceType marketplaceType)
        {
            _logger.LogInformation("Starting category attribute sync for MarketplaceType: {MarketplaceType}", marketplaceType);

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
                IRepository<Category> scopedCategoryRepository = scopedUnitOfWork.GetRepository<Category>();

                List<int> categoryIdsInCurrentBatchList = attributeDownloadResults.Select(downloadResult => (int)downloadResult.CategoryEntity.Id).ToList();

                IList<Category> existingCategoryEntitiesWithAttributes = await scopedCategoryRepository.GetAllAsync(
                    predicate: category => categoryIdsInCurrentBatchList.Contains(category.Id),
                    include: source => source.Include(category => category.Attributes)
                                             .ThenInclude(categoryAttribute => categoryAttribute.Values),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                foreach (var attributeDownloadResult in attributeDownloadResults)
                {
                    Category currentCategoryEntityProxy = attributeDownloadResult.CategoryEntity;
                    List<MarketplaceCategoryAttributeDto> incomingAttributeDtos = attributeDownloadResult.AttributeDtos;

                    Category? managedCategoryEntity = existingCategoryEntitiesWithAttributes.FirstOrDefault(c => c.Id == currentCategoryEntityProxy.Id);

                    if (managedCategoryEntity is null) continue;

                    var distinctIncomingAttributeDtos = incomingAttributeDtos
                        .DistinctBy(attributeDto => attributeDto.ExternalId)
                        .ToList();

                    foreach (MarketplaceCategoryAttributeDto incomingAttributeDto in distinctIncomingAttributeDtos)
                    {
                        CategoryAttribute processedCategoryAttributeEntity = managedCategoryEntity.AddOrUpdateAttribute(
                            incomingAttributeDto.ExternalId,
                            incomingAttributeDto.Name,
                            incomingAttributeDto.IsVariant
                        );

                        if (incomingAttributeDto.Values != null)
                        {
                            foreach (MarketplaceAttributeValueDto incomingValueDto in incomingAttributeDto.Values)
                            {
                                processedCategoryAttributeEntity.AddOrUpdateValue(
                                    incomingValueDto.ExternalId,
                                    incomingValueDto.Value
                                );
                            }
                        }
                    }
                }

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }
    }
}
