using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Business.Integrations.Abstract;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Factories;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Entities;

namespace TKH.Business.Concrete
{
    public class ProductSyncService : IProductSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly MarketplaceProviderFactory _marketplaceProviderFactory;
        private readonly IMapper _mapper;

        private const int BATCH_SIZE_LIMIT = 1000;

        public ProductSyncService(
            IUnitOfWork unitOfWork,
            MarketplaceProviderFactory marketplaceProviderFactory,
            IMarketplaceAccountService marketplaceAccountService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _marketplaceProviderFactory = marketplaceProviderFactory;
            _marketplaceAccountService = marketplaceAccountService;
            _mapper = mapper;
        }

        public async Task SyncProductsFromMarketplaceAsync(MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto)
        {
            IMarketplaceProductProvider marketplaceProductProvider = _marketplaceProviderFactory.GetProvider(marketplaceAccountConnectionDetailsDto.MarketplaceType);

            List<MarketplaceProductDto> marketplaceProductDtoBuffer = new List<MarketplaceProductDto>(BATCH_SIZE_LIMIT);

            await foreach (MarketplaceProductDto incomingProductDto in marketplaceProductProvider.GetProductsStreamAsync(marketplaceAccountConnectionDetailsDto))
            {
                marketplaceProductDtoBuffer.Add(incomingProductDto);

                if (marketplaceProductDtoBuffer.Count >= BATCH_SIZE_LIMIT)
                {
                    await ProcessProductBatchAsync(marketplaceProductDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
                    marketplaceProductDtoBuffer.Clear();
                }
            }

            if (marketplaceProductDtoBuffer.Count > 0)
                await ProcessProductBatchAsync(marketplaceProductDtoBuffer, marketplaceAccountConnectionDetailsDto.Id);
        }

        private async Task ProcessProductBatchAsync(List<MarketplaceProductDto> marketplaceProductDtoList, int marketplaceAccountId)
        {
            List<string> incomingBarcodeList = marketplaceProductDtoList.Select(marketplaceProductDto => marketplaceProductDto.Barcode)
                .Where(marketplaceProductDto => !string.IsNullOrEmpty(marketplaceProductDto))
                .ToList();

            List<Product> existingProductList = await _productRepository.GetAllAsync(product => product.MarketplaceAccountId == marketplaceAccountId && incomingBarcodeList.Contains(product.Barcode));

            List<Product> productsToProcessList = new List<Product>();

            foreach (MarketplaceProductDto marketplaceProductDto in marketplaceProductDtoList)
            {
                Product existingProduct = existingProductList.FirstOrDefault(product => product.Barcode == marketplaceProductDto.Barcode);

                if (existingProduct != null)
                {
                    _mapper.Map(marketplaceProductDto, existingProduct);
                    existingProduct.CommissionRate = 0; //Todo: Kategori servisi ile beraber dahil edilecek.
                    existingProduct.LastUpdateDateTime = DateTime.UtcNow;
                    productsToProcessList.Add(existingProduct);
                }
                else
                {
                    Product newProduct = _mapper.Map<Product>(marketplaceProductDto);
                    newProduct.MarketplaceAccountId = marketplaceAccountId;
                    newProduct.CommissionRate = 0;  //Todo: Kategori servisi ile beraber dahil edilecek.
                    newProduct.LastUpdateDateTime = DateTime.UtcNow;

                    productsToProcessList.Add(newProduct);
                }
            }

            _productRepository.AddOrUpdate(productsToProcessList);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
