using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Core.Utilities.Security.Encryption;
using TKH.Entities;
using TKH.Entities.Enums;

namespace TKH.Business.Concrete
{
    public class MarketplaceAccountService : IMarketplaceAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MarketplaceAccount> _marketplaceAccountRepository;
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly ICipherService _cipherService;

        public MarketplaceAccountService(IUnitOfWork unitOfWork, IMapper mapper, IWorkContext workContext, ICipherService cipherService)
        {
            _unitOfWork = unitOfWork;
            _marketplaceAccountRepository = _unitOfWork.GetRepository<MarketplaceAccount>();
            _mapper = mapper;
            _cipherService = cipherService;
            _workContext = workContext;
        }

        public async Task<IResult> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto)
        {
            MarketplaceAccount marketplaceAccountEntity = _mapper.Map<MarketplaceAccount>(marketplaceAccountAddDto);

            marketplaceAccountEntity.ApiSecretKey = _cipherService.Encrypt(marketplaceAccountAddDto.ApiSecretKey);
            marketplaceAccountEntity.ConnectionState = MarketplaceConnectionState.Initializing;
            marketplaceAccountEntity.SyncState = MarketplaceSyncState.Queued;
            marketplaceAccountEntity.IsActive = true;
            marketplaceAccountEntity.LastErrorMessage = null;

            await _marketplaceAccountRepository.InsertAsync(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Pazar yeri hesabı başarıyla eklendi ve kurulum kuyruğuna alındı.");
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
                    return new ErrorResult("Veri eşitlemesi devam eden bir mağazayı silemezsiniz. Lütfen işlemin bitmesini bekleyin.");
            }

            _marketplaceAccountRepository.Delete(account);
            await _unitOfWork.SaveChangesAsync();

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

            MarketplaceAccountConnectionDetailsDto marketplaceAccountConnectionDetailsDto = _mapper.Map<MarketplaceAccountConnectionDetailsDto>(marketplaceAccountEntity, opt =>
            {
                opt.Items["CipherService"] = _cipherService;
            });

            return new SuccessDataResult<MarketplaceAccountConnectionDetailsDto>(marketplaceAccountConnectionDetailsDto);
        }
    }
}
