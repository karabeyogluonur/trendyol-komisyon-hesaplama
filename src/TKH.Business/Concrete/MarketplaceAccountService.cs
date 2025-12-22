using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Core.Utilities.Security.Encryption;
using TKH.Entities;

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

            await _marketplaceAccountRepository.AddAsync(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Pazar yeri hesabı başarıyla eklendi.");
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetAllAsync()
        {
            List<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync();
            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);
            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountUpdateDto>> GetByIdAsync(int id)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetAsync(marketplaceAccount => marketplaceAccount.Id == id);

            if (marketplaceAccountEntity is null)
                return new ErrorDataResult<MarketplaceAccountUpdateDto>("Kayıt bulunamadı.");

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(marketplaceAccountEntity);

            marketplaceAccountUpdateDto.ApiSecretKey = string.Empty;

            return new SuccessDataResult<MarketplaceAccountUpdateDto>(marketplaceAccountUpdateDto);
        }

        public async Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetAsync(marketplaceAccount => marketplaceAccount.Id == updateDto.Id);

            if (marketplaceAccountEntity is null)
                return new ErrorResult("Düzenlenecek kayıt bulunamadı.");

            marketplaceAccountEntity.StoreName = updateDto.StoreName;
            marketplaceAccountEntity.MerchantId = updateDto.MerchantId;
            marketplaceAccountEntity.ApiKey = updateDto.ApiKey;
            marketplaceAccountEntity.MarketplaceType = updateDto.MarketplaceType;
            marketplaceAccountEntity.IsActive = updateDto.IsActive;

            if (!string.IsNullOrEmpty(updateDto.ApiSecretKey))
                marketplaceAccountEntity.ApiSecretKey = _cipherService.Encrypt(updateDto.ApiSecretKey);

            _marketplaceAccountRepository.Update(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Hesap başarıyla güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            if (_workContext.CurrentMarketplaceAccountId == id)
                return new ErrorResult("Şu anda aktif olarak seçili olan mağazayı silemezsiniz. Lütfen önce başka bir mağazaya geçiş yapın.");

            MarketplaceAccount? account = await _marketplaceAccountRepository.GetAsync(marketplaceAccount => marketplaceAccount.Id == id);

            if (account is null)
                return new ErrorResult("Silinecek hesap bulunamadı.");

            _marketplaceAccountRepository.Delete(account);

            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Pazar yeri hesabı başarıyla silindi.");
        }

        public async Task<IDataResult<List<MarketplaceAccountSummaryDto>>> GetActiveAccountsAsync()
        {
            List<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync(marketplaceAccount => marketplaceAccount.IsActive);
            List<MarketplaceAccountSummaryDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountSummaryDto>>(marketplaceAccountEntities);
            return new SuccessDataResult<List<MarketplaceAccountSummaryDto>>(marketplaceAccountListDtos, "Aktif Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountConnectionDetailsDto>> GetConnectionDetailsByIdAsync(int id)
        {
            MarketplaceAccount? marketplaceAccountEntity = await _marketplaceAccountRepository.GetAsync(marketplaceAccount => marketplaceAccount.Id == id);

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
