using AutoMapper;
using Microsoft.Extensions.Logging;
using TKH.Business.Features.MarketplaceAccounts.Dtos;
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
            _cipherService = cipherService;
            _workContext = workContext;
            _logger = logger;
        }

        public async Task<IDataResult<int>> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto)
        {
            MarketplaceAccount marketplaceAccountEntity = _mapper.Map<MarketplaceAccount>(marketplaceAccountAddDto);

            marketplaceAccountEntity.ApiSecretKey = _cipherService.Encrypt(marketplaceAccountAddDto.ApiSecretKey);
            marketplaceAccountEntity.ConnectionState = MarketplaceConnectionState.Initializing;
            marketplaceAccountEntity.SyncState = MarketplaceSyncState.Queued;
            marketplaceAccountEntity.IsActive = true;
            marketplaceAccountEntity.LastErrorMessage = null;

            await _marketplaceAccountRepository.InsertAsync(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New Marketplace Account created. Id: {AccountId}, Type: {MarketplaceType}", marketplaceAccountEntity.Id, marketplaceAccountEntity.MarketplaceType);

            return new SuccessDataResult<int>(marketplaceAccountEntity.Id, "Pazar yeri hesabı başarıyla eklendi ve kurulum kuyruğuna alındı.");
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllAsync()
        {
            IList<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync();

            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);

            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountDetailsDto>> GetByIdAsync(int id)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == id);

            if (marketplaceAccountEntity is null)
                return new ErrorDataResult<MarketplaceAccountDetailsDto>("Kayıt bulunamadı.");

            MarketplaceAccountDetailsDto marketplaceAccountDetailsDto = _mapper.Map<MarketplaceAccountDetailsDto>(marketplaceAccountEntity);

            marketplaceAccountDetailsDto.ApiSecretKey = string.Empty;

            return new SuccessDataResult<MarketplaceAccountDetailsDto>(marketplaceAccountDetailsDto);
        }

        public async Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == updateDto.Id);

            if (marketplaceAccountEntity is null)
                return new ErrorResult("Düzenlenecek kayıt bulunamadı.");

            if (marketplaceAccountEntity.SyncState == MarketplaceSyncState.Syncing)
            {
                bool isZombieLock = marketplaceAccountEntity.LastSyncStartTime.HasValue &&
                                    marketplaceAccountEntity.LastSyncStartTime.Value < DateTime.UtcNow.AddHours(-2);

                if (!isZombieLock)
                {
                    _logger.LogWarning("Update attempt blocked for Account {AccountId}. Sync is in progress.", updateDto.Id);
                    return new ErrorResult("Bu mağaza şu anda veri eşitleme işlemi yaptığı için düzenlenemez. Lütfen işlem bitince tekrar deneyin.");
                }
            }

            bool isCredentialsChanged = marketplaceAccountEntity.ApiKey != updateDto.ApiKey ||
                                    marketplaceAccountEntity.MerchantId != updateDto.MerchantId ||
                                    (!string.IsNullOrEmpty(updateDto.ApiSecretKey));

            marketplaceAccountEntity.StoreName = updateDto.StoreName;
            marketplaceAccountEntity.MerchantId = updateDto.MerchantId;
            marketplaceAccountEntity.ApiKey = updateDto.ApiKey;
            marketplaceAccountEntity.MarketplaceType = updateDto.MarketplaceType;
            marketplaceAccountEntity.IsActive = updateDto.IsActive;

            if (!string.IsNullOrEmpty(updateDto.ApiSecretKey))
                marketplaceAccountEntity.ApiSecretKey = _cipherService.Encrypt(updateDto.ApiSecretKey);


            if (isCredentialsChanged)
            {
                marketplaceAccountEntity.ConnectionState = MarketplaceConnectionState.Initializing;
                marketplaceAccountEntity.LastErrorMessage = null;
                marketplaceAccountEntity.LastErrorDate = null;
                _logger.LogInformation("Credentials changed for Account {AccountId}. Connection state reset to Initializing.", updateDto.Id);
            }

            _marketplaceAccountRepository.Update(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            string resultMessage = isCredentialsChanged
                ? "Hesap güncellendi ve bağlantı ayarları değiştiği için yeniden doğrulama kuyruğuna alındı."
                : "Hesap başarıyla güncellendi.";

            return new SuccessResult(resultMessage);
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            if (_workContext.CurrentMarketplaceAccountId == id)
                return new ErrorResult("Şu anda aktif olarak seçili olan mağazayı silemezsiniz. Lütfen önce başka bir mağazaya geçiş yapın.");

            MarketplaceAccount account = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == id);

            if (account is null)
                return new ErrorResult("Silinecek hesap bulunamadı.");

            if (account.SyncState == MarketplaceSyncState.Syncing)
            {
                bool isZombieLock = account.LastSyncStartTime.HasValue && account.LastSyncStartTime.Value < DateTime.UtcNow.AddHours(-2);

                if (!isZombieLock)
                {
                    _logger.LogWarning("Delete attempt blocked for Account {AccountId}. Sync is in progress.", id);
                    return new ErrorResult("Veri eşitlemesi devam eden bir mağazayı silemezsiniz. Lütfen işlemin bitmesini bekleyin.");
                }
            }

            _marketplaceAccountRepository.Delete(account);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Marketplace Account deleted. Id: {AccountId}", id);

            return new SuccessResult("Pazar yeri hesabı başarıyla silindi.");
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveAccountsAsync()
        {
            IList<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync(predicate: marketplaceAccount => marketplaceAccount.IsActive);

            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);

            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Aktif Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int id)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == id);

            if (marketplaceAccountEntity is null)
                return new ErrorDataResult<MarketplaceAccountConnectionDetailsDto>("Bağlantı bilgileri için gerekli hesap bulunamadı.");

            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = _mapper.Map<MarketplaceAccountConnectionDetailsDto>(marketplaceAccountEntity);

            if (!string.IsNullOrEmpty(marketplaceAccountEntity.ApiSecretKey))
            {
                marketplaceAccountConnectionDetailsDto = marketplaceAccountConnectionDetailsDto with
                {
                    ApiSecretKey = _cipherService.Decrypt(marketplaceAccountEntity.ApiSecretKey)
                };
            }

            return new SuccessDataResult<MarketplaceAccountConnectionDetailsDto>(marketplaceAccountConnectionDetailsDto);
        }

        public async Task<bool> TryMarkAsSyncingAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId, disableTracking: false);

            if (marketplaceAccount is null) return false;

            bool isStuck = marketplaceAccount.SyncState == MarketplaceSyncState.Syncing &&
                           marketplaceAccount.LastSyncStartTime.HasValue &&
                           marketplaceAccount.LastSyncStartTime.Value < DateTime.UtcNow.AddHours(-2);

            if (isStuck)
            {
                _logger.LogWarning("Zombie lock detected for Account {AccountId}. Breaking lock and restarting sync.", marketplaceAccountId);
            }

            bool isAvailable = marketplaceAccount.SyncState == MarketplaceSyncState.Idle ||
                               marketplaceAccount.SyncState == MarketplaceSyncState.Queued;

            if (!isAvailable && !isStuck)
                return false;

            marketplaceAccount.SyncState = MarketplaceSyncState.Syncing;
            marketplaceAccount.LastSyncStartTime = DateTime.UtcNow;

            if (marketplaceAccount.ConnectionState == MarketplaceConnectionState.AuthError ||
                marketplaceAccount.ConnectionState == MarketplaceConnectionState.SystemError)
            {
                marketplaceAccount.ConnectionState = MarketplaceConnectionState.Initializing;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sync lock acquired for Account {AccountId}. Sync initiated.", marketplaceAccountId);

            return true;
        }

        public async Task MarkSyncCompletedAsync(int marketplaceAccountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId);

            if (marketplaceAccount is null) return;

            marketplaceAccount.SyncState = MarketplaceSyncState.Idle;
            marketplaceAccount.ConnectionState = MarketplaceConnectionState.Connected;
            marketplaceAccount.LastErrorMessage = null;
            marketplaceAccount.LastErrorDate = null;

            _marketplaceAccountRepository.Update(marketplaceAccount);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sync cycle finished successfully for Account {AccountId}.", marketplaceAccountId);
        }

        public async Task MarkSyncFailedAsync(int marketplaceAccountId, Exception exception)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(predicate: marketplaceAccount => marketplaceAccount.Id == marketplaceAccountId);

            if (marketplaceAccount == null) return;

            marketplaceAccount.SyncState = MarketplaceSyncState.Idle;
            marketplaceAccount.LastErrorMessage = exception.Message;
            marketplaceAccount.LastErrorDate = DateTime.UtcNow;

            if (exception is IntegrationAuthException)
                marketplaceAccount.ConnectionState = MarketplaceConnectionState.AuthError;
            else
                marketplaceAccount.ConnectionState = MarketplaceConnectionState.SystemError;

            _marketplaceAccountRepository.Update(marketplaceAccount);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogError(exception, "Sync cycle failed for Account {AccountId}. ConnectionState set to {ConnectionState}.", marketplaceAccountId, marketplaceAccount.ConnectionState);
        }

        public async Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsAsync(int accountId)
        {
            MarketplaceAccount marketplaceAccount = await _marketplaceAccountRepository.GetFirstOrDefaultAsync(
                predicate: marketplaceAccount => marketplaceAccount.Id == accountId,
                disableTracking: true
            );

            if (marketplaceAccount is null)
                return new ErrorDataResult<MarketplaceAccountConnectionDetailsDto>(null, "Hesap bulunamadı");

            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = _mapper.Map<MarketplaceAccountConnectionDetailsDto>(marketplaceAccount);

            if (!string.IsNullOrEmpty(marketplaceAccount.ApiSecretKey))
            {
                marketplaceAccountConnectionDetailsDto = marketplaceAccountConnectionDetailsDto with
                {
                    ApiSecretKey = _cipherService.Decrypt(marketplaceAccount.ApiSecretKey)
                };
            }

            return new SuccessDataResult<MarketplaceAccountConnectionDetailsDto>(marketplaceAccountConnectionDetailsDto);
        }

        public async Task<IDataResult<IList<int>>> GetActiveConnectedAccountIdsAsync()
        {
            IList<int> accountIds = await _marketplaceAccountRepository.GetAllAsync(
                selector: marketplaceAccount => marketplaceAccount.Id,
                predicate: marketplaceAccount => marketplaceAccount.IsActive && marketplaceAccount.ConnectionState == MarketplaceConnectionState.Connected,
                disableTracking: true
            );

            return new SuccessDataResult<IList<int>>(accountIds);
        }
    }
}
