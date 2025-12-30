using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.Common.Constants;
using TKH.Core.DataAccess;
using TKH.Entities;

namespace TKH.Business.Concrete
{
    public class ClaimSyncService : IClaimSyncService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        public ClaimSyncService(
            IServiceScopeFactory serviceScopeFactory,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _mapper = mapper;
        }

        public async Task SyncClaimsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceClaimProvider marketplaceClaimProvider = _marketplaceProviderFactory.GetProvider<IMarketplaceClaimProvider>(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceClaimDto> marketplaceClaimDtoBuffer = new List<MarketplaceClaimDto>(ApplicationDefaults.ClaimBatchSize);

            await foreach (MarketplaceClaimDto incomingMarketplaceClaimDto in marketplaceClaimProvider.GetClaimsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceClaimDtoBuffer.Add(incomingMarketplaceClaimDto);

                if (marketplaceClaimDtoBuffer.Count >= ApplicationDefaults.ClaimBatchSize)
                {
                    await ProcessClaimBatchAsync(marketplaceClaimDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceClaimDtoBuffer.Clear();
                }
            }

            if (marketplaceClaimDtoBuffer.Count > 0)
                await ProcessClaimBatchAsync(marketplaceClaimDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
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

                Dictionary<string, int> barcodeToLocalIdMap = relatedProductList
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

                List<Claim> newClaimsToAdd = new List<Claim>();

                foreach (MarketplaceClaimDto marketplaceClaimDto in marketplaceClaimDtoList)
                {
                    Claim? existingClaim = existingClaimList.FirstOrDefault(claim => claim.ExternalId == marketplaceClaimDto.ExternalId);

                    if (existingClaim is not null)
                    {
                        _mapper.Map(marketplaceClaimDto, existingClaim);
                        existingClaim.LastUpdateDateTime = DateTime.UtcNow;

                        SyncClaimItems(existingClaim, marketplaceClaimDto.Items, barcodeToLocalIdMap);
                    }
                    else
                    {
                        Claim newClaim = _mapper.Map<Claim>(marketplaceClaimDto);
                        newClaim.MarketplaceAccountId = marketplaceAccountId;
                        newClaim.LastUpdateDateTime = DateTime.UtcNow;

                        SyncClaimItems(newClaim, marketplaceClaimDto.Items, barcodeToLocalIdMap);

                        newClaimsToAdd.Add(newClaim);
                    }
                }

                if (newClaimsToAdd.Count > 0)
                    await scopedClaimRepository.InsertAsync(newClaimsToAdd);

                await scopedUnitOfWork.SaveChangesAsync();
            }
        }

        private void SyncClaimItems(
            Claim claim,
            List<MarketplaceClaimItemDto> marketplaceItems,
            Dictionary<string, int> barcodeToLocalIdMap)
        {
            if (marketplaceItems is null || !marketplaceItems.Any())
                return;

            if (claim.ClaimItems is null)
                claim.ClaimItems = new List<ClaimItem>();
            else
                claim.ClaimItems.Clear();

            foreach (MarketplaceClaimItemDto itemDto in marketplaceItems)
            {
                ClaimItem claimItem = _mapper.Map<ClaimItem>(itemDto);

                if (!string.IsNullOrEmpty(itemDto.Barcode) && barcodeToLocalIdMap.TryGetValue(itemDto.Barcode, out int productId))
                    claimItem.ProductId = productId;
                else
                    claimItem.ProductId = null;

                claim.ClaimItems.Add(claimItem);
            }
        }
    }
}
