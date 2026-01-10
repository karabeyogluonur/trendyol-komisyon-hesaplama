using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Extensions;
using TKH.Core.DataAccess;
using TKH.Entities;

namespace TKH.Business.Features.Claims.Services
{
    public class ClaimSyncService : IClaimSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly ILogger<ClaimSyncService> _logger;

        public ClaimSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            ILogger<ClaimSyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _logger = logger;
        }

        public async Task SyncClaimsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            _logger.LogInformation("Starting claim sync for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);

            IMarketplaceClaimProvider marketplaceClaimProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceClaimProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceClaimDto> marketplaceClaimDtoBufferList = new List<MarketplaceClaimDto>(ApplicationDefaults.ClaimBatchSize);

            await foreach (MarketplaceClaimDto incomingMarketplaceClaimDto in marketplaceClaimProvider.GetClaimsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceClaimDtoBufferList.Add(incomingMarketplaceClaimDto);

                if (marketplaceClaimDtoBufferList.Count >= ApplicationDefaults.ClaimBatchSize)
                {
                    await ProcessClaimBatchAsync(marketplaceClaimDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceClaimDtoBufferList.Clear();
                }
            }

            if (marketplaceClaimDtoBufferList.Count > 0)
                await ProcessClaimBatchAsync(marketplaceClaimDtoBufferList, marketplaceAccountConnectionDetailsDto.Id);

            _logger.LogInformation("Claim sync completed for MarketplaceAccount: {AccountId}", marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessClaimBatchAsync(List<MarketplaceClaimDto> marketplaceClaimDtoList, int marketplaceAccountId)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IRepository<Claim> scopedClaimRepository = scopedUnitOfWork.GetRepository<Claim>();
                IRepository<Product> scopedProductRepository = scopedUnitOfWork.GetRepository<Product>();

                List<string> incomingExternalIdList = marketplaceClaimDtoList
                    .Select(dto => dto.ExternalId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                List<string> allMarketplaceBarcodes = marketplaceClaimDtoList
                    .SelectMany(dto => dto.Items)
                    .Select(item => item.Barcode)
                    .Where(barcode => !string.IsNullOrEmpty(barcode))
                    .Distinct()
                    .ToList();

                IList<Product> relatedProductList = await scopedProductRepository.GetAllAsync(
                    predicate: product => product.MarketplaceAccountId == marketplaceAccountId &&
                                          allMarketplaceBarcodes.Contains(product.Barcode),
                    disableTracking: true,
                    ignoreQueryFilters: true
                );

                Dictionary<string, int> barcodeToLocalIdMapDictionary = relatedProductList
                    .Where(product => !string.IsNullOrEmpty(product.Barcode))
                    .GroupBy(product => product.Barcode)
                    .ToDictionary(group => group.Key, group => group.First().Id);

                IList<Claim> existingClaimList = await scopedClaimRepository.GetAllAsync(
                    predicate: claim => claim.MarketplaceAccountId == marketplaceAccountId &&
                                        incomingExternalIdList.Contains(claim.ExternalId),
                    include: source => source.Include(claim => claim.ClaimItems),
                    disableTracking: false,
                    ignoreQueryFilters: true
                );

                List<Claim> newClaimsToAddList = new List<Claim>();

                foreach (MarketplaceClaimDto marketplaceClaimDto in marketplaceClaimDtoList)
                {
                    Claim? existingClaimEntity = existingClaimList.FirstOrDefault(claim => claim.ExternalId == marketplaceClaimDto.ExternalId);

                    List<ClaimItem> claimItemsForSync = CreateClaimItemsFromDto(
                        existingClaimEntity?.Id ?? 0,
                        marketplaceClaimDto.Items,
                        barcodeToLocalIdMapDictionary
                    );

                    if (existingClaimEntity is not null)
                    {
                        existingClaimEntity.UpdateDetails(
                            marketplaceClaimDto.CustomerFirstName,
                            marketplaceClaimDto.CustomerLastName,
                            marketplaceClaimDto.CargoTrackingNumber,
                            marketplaceClaimDto.CargoProviderName,
                            marketplaceClaimDto.CargoSenderNumber,
                            marketplaceClaimDto.CargoTrackingLink,
                            marketplaceClaimDto.RejectedExternalPackageId,
                            marketplaceClaimDto.RejectedCargoTrackingNumber,
                            marketplaceClaimDto.RejectedCargoProviderName,
                            marketplaceClaimDto.RejectedCargoTrackingLink,
                            DateTime.UtcNow
                        );

                        existingClaimEntity.SyncItems(claimItemsForSync);
                    }
                    else
                    {
                        Claim newClaimEntity = Claim.Create(
                            marketplaceAccountId,
                            marketplaceClaimDto.ExternalId,
                            marketplaceClaimDto.ExternalOrderNumber,
                            marketplaceClaimDto.ExternalShipmentPackageId,
                            marketplaceClaimDto.ClaimDate.EnsureUtc(),
                            marketplaceClaimDto.OrderDate.EnsureUtc(),
                            marketplaceClaimDto.CustomerFirstName,
                            marketplaceClaimDto.CustomerLastName,
                            marketplaceClaimDto.CargoTrackingNumber,
                            marketplaceClaimDto.CargoProviderName,
                            marketplaceClaimDto.CargoSenderNumber,
                            marketplaceClaimDto.CargoTrackingLink,
                            marketplaceClaimDto.RejectedExternalPackageId,
                            marketplaceClaimDto.RejectedCargoTrackingNumber,
                            marketplaceClaimDto.RejectedCargoProviderName,
                            marketplaceClaimDto.RejectedCargoTrackingLink
                        );

                        newClaimEntity.SyncItems(claimItemsForSync);
                        newClaimsToAddList.Add(newClaimEntity);
                    }
                }

                if (newClaimsToAddList.Count > 0)
                    await scopedClaimRepository.InsertAsync(newClaimsToAddList);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private List<ClaimItem> CreateClaimItemsFromDto(int claimId, List<MarketplaceClaimItemDto> marketplaceClaimItemDtos, Dictionary<string, int> barcodeToLocalIdMapDictionary)
        {
            List<ClaimItem> claimItemEntities = new List<ClaimItem>();

            if (marketplaceClaimItemDtos is null || !marketplaceClaimItemDtos.Any())
                return claimItemEntities;

            foreach (MarketplaceClaimItemDto marketplaceClaimItemDto in marketplaceClaimItemDtos)
            {
                int? matchedProductId = null;
                if (!string.IsNullOrEmpty(marketplaceClaimItemDto.Barcode) && barcodeToLocalIdMapDictionary.TryGetValue(marketplaceClaimItemDto.Barcode, out int productId))
                    matchedProductId = productId;

                ClaimItem claimItemEntity = ClaimItem.Create(
                    claimId,
                    matchedProductId,
                    marketplaceClaimItemDto.ExternalId,
                    marketplaceClaimItemDto.ExternalOrderLineItemId,
                    marketplaceClaimItemDto.Barcode,
                    marketplaceClaimItemDto.Sku,
                    marketplaceClaimItemDto.ProductName,
                    marketplaceClaimItemDto.Price,
                    marketplaceClaimItemDto.VatRate,
                    marketplaceClaimItemDto.Status,
                    marketplaceClaimItemDto.CustomerNote,
                    marketplaceClaimItemDto.ReasonType,
                    marketplaceClaimItemDto.ReasonName,
                    marketplaceClaimItemDto.ReasonCode,
                    marketplaceClaimItemDto.IsResolved,
                    marketplaceClaimItemDto.IsAutoAccepted,
                    marketplaceClaimItemDto.IsAcceptedBySeller
                );

                claimItemEntities.Add(claimItemEntity);
            }

            return claimItemEntities;
        }
    }
}
