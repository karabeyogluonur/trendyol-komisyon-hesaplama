using AutoMapper;

using Microsoft.Extensions.Logging;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Core.Common.Constants;
using TKH.Core.Common.Exceptions;
using TKH.Core.Contexts;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Core.Utilities.Security.Encryption;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Features.MarketplaceAccounts.Services
{
    public class MarketplaceAccountService : IMarketplaceAccountService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MarketplaceAccount> _marketplaceAccountRepository;
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly ICipherService _cipherService;
        private readonly ILogger<MarketplaceAccountService> _logger;

        public MarketplaceAccountService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWorkContext workContext,
            ICipherService cipherService,
            ILogger<MarketplaceAccountService> logger)
        {
            _unitOfWork = unitOfWork;
            _marketplaceAccountRepository = _unitOfWork.GetRepository<MarketplaceAccount>();
            _mapper = mapper;
            _workContext = workContext;
            _cipherService = cipherService;
            _logger = logger;
        }

        public async Task<IDataResult<int>> CreateMarketplaceAccountAsync(MarketplaceAccountCreateDto marketplaceAccountCreateDto)
        {
            bool isAlreadyExists = await _marketplaceAccountRepository.ExistsAsync(marketplaceAccount =>
                    marketplaceAccount.MarketplaceType == marketplaceAccountCreateDto.MarketplaceType &&
                    marketplaceAccount.MerchantId == marketplaceAccountCreateDto.MerchantId
            );

            if (isAlreadyExists)
                return new ErrorDataResult<int>("Bu pazar yeri için bu satıcı zaten eklenmiş.");

            MarketplaceAccount marketplaceAccountEntity = MarketplaceAccount.Create(
                marketplaceAccountCreateDto.MarketplaceType,
                marketplaceAccountCreateDto.StoreName,
                marketplaceAccountCreateDto.MerchantId,
                marketplaceAccountCreateDto.ApiKey,
                _cipherService.Encrypt(marketplaceAccountCreateDto.ApiSecretKey)
            );

            await _marketplaceAccountRepository.InsertAsync(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New Marketplace Account created. Id: {AccountId}, Type: {MarketplaceType}", marketplaceAccountEntity.Id, marketplaceAccountEntity.MarketplaceType);

            return new SuccessDataResult<int>(marketplaceAccountEntity.Id, "Pazar yeri hesabı başarıyla eklendi ve kurulum kuyruğuna alındı.");
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllMarketplaceAccountsAsync()
        {
            IList<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync();

            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos =
                _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);

            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountDetailsDto>> GetMarketplaceAccountByIdAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId
                );

            if (marketplaceAccountEntity is null)
                return new ErrorDataResult<MarketplaceAccountDetailsDto>("Kayıt bulunamadı.");

            MarketplaceAccountDetailsDto marketplaceAccountDetailsDto = _mapper.Map<MarketplaceAccountDetailsDto>(marketplaceAccountEntity);

            marketplaceAccountDetailsDto.ApiSecretKey = string.Empty;

            return new SuccessDataResult<MarketplaceAccountDetailsDto>(
                marketplaceAccountDetailsDto
            );
        }

        public async Task<IResult> UpdateMarketplaceAccountAsync(MarketplaceAccountUpdateDto marketplaceAccountUpdateDto)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountUpdateDto.Id);

            if (marketplaceAccountEntity is null)
                return new ErrorResult("Düzenlenecek kayıt bulunamadı.");

            bool isDuplicateExists = await _marketplaceAccountRepository.ExistsAsync(marketplaceAccount =>
                    marketplaceAccount.Id != marketplaceAccountUpdateDto.Id &&
                    marketplaceAccount.MarketplaceType == marketplaceAccountUpdateDto.MarketplaceType &&
                    marketplaceAccount.MerchantId == marketplaceAccountUpdateDto.MerchantId
            );

            if (isDuplicateExists)
                return new ErrorResult("Bu pazar yeri için bu satıcı zaten tanımlı.");

            if (marketplaceAccountEntity.SyncState == MarketplaceSyncState.Syncing && !marketplaceAccountEntity.CanStartSync(DateTime.UtcNow, TimeSpan.FromHours(ApplicationDefaults.ZombieSyncThreshold)))
            {
                _logger.LogWarning("Update attempt blocked for Account {AccountId}. Sync is in progress.", marketplaceAccountUpdateDto.Id);

                return new ErrorResult("Bu mağaza şu anda veri eşitleme işlemi yaptığı için düzenlenemez. Lütfen işlem bitince tekrar deneyin.");
            }

            _logger.LogInformation("Updating Marketplace Account {AccountId}", marketplaceAccountUpdateDto.Id);

            marketplaceAccountEntity.UpdateGeneralInfo(marketplaceAccountUpdateDto.StoreName, marketplaceAccountUpdateDto.MarketplaceType, marketplaceAccountUpdateDto.IsActive);
            marketplaceAccountEntity.UpdateCredentials(marketplaceAccountUpdateDto.ApiKey, marketplaceAccountUpdateDto.MerchantId, string.IsNullOrWhiteSpace(marketplaceAccountUpdateDto.ApiSecretKey) ? null : _cipherService.Encrypt(marketplaceAccountUpdateDto.ApiSecretKey));

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Marketplace Account {AccountId} updated successfully", marketplaceAccountUpdateDto.Id);

            return new SuccessResult("Hesap başarıyla güncellendi.");
        }

        public async Task<IResult> DeleteMarketplaceAccountByIdAsync(int marketplaceAccountId)
        {
            if (_workContext.CurrentMarketplaceAccountId == marketplaceAccountId)
                return new ErrorResult("Şu anda aktif olarak seçili olan mağazayı silemezsiniz. Lütfen önce başka bir mağazaya geçiş yapın.");

            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId
                );

            if (marketplaceAccount is null)
                return new ErrorResult("Silinecek hesap bulunamadı.");

            try
            {
                marketplaceAccount.EnsureDeletable(ApplicationDefaults.DemoAccountMerchantId);
            }
            catch (DomainException domainException)
            {
                _logger.LogWarning("Delete blocked for Marketplace Account {AccountId} - Reason: {Reason}", marketplaceAccountId, domainException.Message);
                return new ErrorResult(domainException.Message);
            }

            _marketplaceAccountRepository.Delete(marketplaceAccount);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Marketplace Account deleted. Id: {AccountId}", marketplaceAccountId);

            return new SuccessResult("Pazar yeri hesabı başarıyla silindi.");
        }

        public async Task<bool> TryMarkMarketplaceAccountAsSyncingAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId,
                    disableTracking: false
                );

            if (marketplaceAccount is null)
            {
                _logger.LogWarning("Sync attempt failed: Account not found. Id: {AccountId}", marketplaceAccountId);
                return false;
            }

            if (!marketplaceAccount.CanStartSync(DateTime.UtcNow, TimeSpan.FromHours(ApplicationDefaults.ZombieSyncThreshold)))
            {
                _logger.LogInformation("Sync attempt skipped for Account {AccountId} - Account not ready or locked", marketplaceAccountId);
                return false;
            }

            marketplaceAccount.MarkAsSyncing(DateTime.UtcNow);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sync lock acquired for Account {AccountId}. Sync initiated.", marketplaceAccountId);

            return true;
        }

        public async Task MarkMarketplaceAccountSyncCompletedAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId
                );

            if (marketplaceAccount is null)
            {
                _logger.LogWarning("Sync completed skipped: Account not found. Id: {AccountId}", marketplaceAccountId);
                return;
            }

            marketplaceAccount.MarkSyncCompleted();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sync cycle finished successfully for Account {AccountId}.", marketplaceAccountId);
        }

        public async Task MarkMarketplaceAccountSyncFailedAsync(int marketplaceAccountId, Exception exception)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId
                );

            if (marketplaceAccount is null)
            {
                _logger.LogWarning("Sync failed logging skipped: Account not found. Id: {AccountId}", marketplaceAccountId);
                return;
            }

            bool isAuthError = exception is IntegrationAuthException;

            marketplaceAccount.MarkSyncFailed(exception.Message, isAuthError);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogError(exception, "Sync cycle failed for Account {AccountId}.", marketplaceAccountId);
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveMarketplaceAccountsAsync()
        {
            IList<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync(
                    predicate: marketplaceAccount => marketplaceAccount.IsActive,
                    disableTracking: true
                );

            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);

            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Aktif pazar yeri hesapları listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                    predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId,
                    disableTracking: true
                );

            if (marketplaceAccount is null)
                return new ErrorDataResult<MarketplaceAccountConnectionDetailsDto>(
                    "Hesap bulunamadı."
                );

            MarketplaceAccountConnectionDetailsDto connectionDetailsDto = _mapper.Map<MarketplaceAccountConnectionDetailsDto>(marketplaceAccount);

            if (!string.IsNullOrWhiteSpace(marketplaceAccount.ApiSecretKey))
            {
                connectionDetailsDto = connectionDetailsDto with
                {
                    ApiSecretKey = _cipherService.Decrypt(marketplaceAccount.ApiSecretKey)
                };
            }

            return new SuccessDataResult<MarketplaceAccountConnectionDetailsDto>(
                connectionDetailsDto
            );
        }

        public async Task<IDataResult<IList<int>>> GetActiveConnectedMarketplaceAccountIdsAsync()
        {
            IList<int> acticeConnectedMarketplaceAccountIds = await _marketplaceAccountRepository.GetAllAsync(
                    selector: marketplaceAccount => marketplaceAccount.Id,
                    predicate: marketplaceAccount => marketplaceAccount.IsActive && marketplaceAccount.ConnectionState == MarketplaceConnectionState.Connected,
                    disableTracking: true
                );

            return new SuccessDataResult<IList<int>>(acticeConnectedMarketplaceAccountIds);
        }

    }
}
