using AutoMapper;
using TKH.Business.Abstract;
using TKH.Business.Dtos.MarketplaceAccount;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Results;
using TKH.Core.Utilities.Security.Encryption;
using TKH.Entities.Concrete;

namespace TKH.Business.Concrete
{
    public class MarketplaceAccountService : IMarketplaceAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MarketplaceAccount> _marketplaceAccountRepository;
        private readonly IMapper _mapper;
        private readonly ICipherService _cipherService;

        public MarketplaceAccountService(IUnitOfWork unitOfWork, IMapper mapper, ICipherService cipherService)
        {
            _unitOfWork = unitOfWork;
            _marketplaceAccountRepository = _unitOfWork.GetRepository<MarketplaceAccount>();
            _mapper = mapper;
            _cipherService = cipherService;
        }

        public async Task<IResult> AddAsync(MarketplaceAccountAddDto marketplaceAccountAddDto)
        {
            MarketplaceAccount marketplaceAccountEntity = _mapper.Map<MarketplaceAccount>(marketplaceAccountAddDto);

            marketplaceAccountEntity.ApiSecretKey = _cipherService.Encrypt(marketplaceAccountAddDto.ApiSecretKey);

            await _marketplaceAccountRepository.AddAsync(marketplaceAccountEntity);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Pazar yeri hesabı başarıyla eklendi.");
        }

        public async Task<IDataResult<List<MarketplaceAccountListDto>>> GetAllAsync()
        {
            List<MarketplaceAccount> marketplaceAccountEntities = await _marketplaceAccountRepository.GetAllAsync();
            List<MarketplaceAccountListDto> marketplaceAccountListDtos = _mapper.Map<List<MarketplaceAccountListDto>>(marketplaceAccountEntities);
            return new SuccessDataResult<List<MarketplaceAccountListDto>>(marketplaceAccountListDtos, "Hesaplar listelendi.");
        }

        public async Task<IDataResult<MarketplaceAccountUpdateDto>> GetByIdAsync(int id)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetAsync(x => x.Id == id);

            if (marketplaceAccountEntity is null)
                return new ErrorDataResult<MarketplaceAccountUpdateDto>("Kayıt bulunamadı.");

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(marketplaceAccountEntity);

            marketplaceAccountUpdateDto.ApiSecretKey = string.Empty;

            return new SuccessDataResult<MarketplaceAccountUpdateDto>(marketplaceAccountUpdateDto);
        }

        public async Task<IResult> UpdateAsync(MarketplaceAccountUpdateDto updateDto)
        {
            MarketplaceAccount marketplaceAccountEntity = await _marketplaceAccountRepository.GetAsync(x => x.Id == updateDto.Id);

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
            var account = await _marketplaceAccountRepository.GetAsync(x => x.Id == id);

            if (account is null)
                return new ErrorResult("Silinecek hesap bulunamadı.");

            _marketplaceAccountRepository.Delete(account);

            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Pazar yeri hesabı başarıyla silindi.");
        }
    }
}
