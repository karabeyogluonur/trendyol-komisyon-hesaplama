using AutoMapper;

using TKH.Business.Features.MarketplaceAccounts.Dtos;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Jobs.Services;
using TKH.Core.Common.Constants;
using TKH.Core.Utilities.Results;
using TKH.Web.Features.MarketplaceAccounts.Models;

using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Web.Features.MarketplaceAccounts.Services
{
    public class MarketplaceAccountOrchestrator : IMarketplaceAccountOrchestrator
    {
        private readonly IMarketplaceAccountService _marketplaceAccountService;
        private readonly IMarketplaceJobService _marketplaceJobService;
        private readonly IMapper _mapper;

        public MarketplaceAccountOrchestrator(
            IMarketplaceAccountService marketplaceAccountService,
            IMarketplaceJobService marketplaceJobService,
            IMapper mapper)
        {
            _marketplaceAccountService = marketplaceAccountService;
            _marketplaceJobService = marketplaceJobService;
            _mapper = mapper;
        }

        public async Task<IDataResult<List<MarketplaceAccountListViewModel>>> PrepareMarketplaceAccountListViewModelAsync()
        {
            IDataResult<List<MarketplaceAccountSummaryDto>> marketplaceAccountSummaryDtoResult = await _marketplaceAccountService.GetAllMarketplaceAccountsAsync();

            if (!marketplaceAccountSummaryDtoResult.Success)
                return new ErrorDataResult<List<MarketplaceAccountListViewModel>>(marketplaceAccountSummaryDtoResult.Message);

            return new SuccessDataResult<List<MarketplaceAccountListViewModel>>(_mapper.Map<List<MarketplaceAccountListViewModel>>(marketplaceAccountSummaryDtoResult.Data));
        }

        public MarketplaceAccountAddViewModel PrepareMarketplaceAccountAddViewModel()
        {
            return new MarketplaceAccountAddViewModel();
        }

        public async Task<IResult> CreateMarketplaceAccountAsync(MarketplaceAccountAddViewModel marketplaceAccountAddViewModel)
        {
            MarketplaceAccountCreateDto marketplaceAccountAddDto = _mapper.Map<MarketplaceAccountCreateDto>(marketplaceAccountAddViewModel);
            IDataResult<int> addMarketplaceAccountResult = await _marketplaceAccountService.CreateMarketplaceAccountAsync(marketplaceAccountAddDto);

            if (!addMarketplaceAccountResult.Success)
                return new ErrorResult(addMarketplaceAccountResult.Message);

            _marketplaceJobService.DispatchImmediateSingleAccountDataSync(addMarketplaceAccountResult.Data);

            return new SuccessResult(addMarketplaceAccountResult.Message);
        }

        public async Task<IDataResult<MarketplaceAccountUpdateViewModel>> PrepareMarketplaceAccountUpdateViewModelAsync(int marketplaceAccountId)
        {
            IDataResult<MarketplaceAccountDetailsDto> getMarketplaceAccountResult = await _marketplaceAccountService.GetMarketplaceAccountByIdAsync(marketplaceAccountId);

            if (!getMarketplaceAccountResult.Success || getMarketplaceAccountResult.Data == null)
                return new ErrorDataResult<MarketplaceAccountUpdateViewModel>(getMarketplaceAccountResult.Message);

            if (getMarketplaceAccountResult.Data.MerchantId == ApplicationDefaults.DemoAccountMerchantId)
            {
                getMarketplaceAccountResult.Data.ApiKey = "*****";
                getMarketplaceAccountResult.Data.ApiKey = "*****";
            }


            return new SuccessDataResult<MarketplaceAccountUpdateViewModel>(_mapper.Map<MarketplaceAccountUpdateViewModel>(getMarketplaceAccountResult.Data));
        }

        public async Task<IResult> UpdateMarketplaceAccountAsync(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            if (marketplaceAccountUpdateViewModel.MerchantId == ApplicationDefaults.DemoAccountMerchantId || marketplaceAccountUpdateViewModel.IsDemo == true)
                return new ErrorResult("Demo hesap düzenlenemez!");

            MarketplaceAccountUpdateDto marketplaceAccountUpdateDto = _mapper.Map<MarketplaceAccountUpdateDto>(marketplaceAccountUpdateViewModel);
            IResult updateMarketplaceAccountResult = await _marketplaceAccountService.UpdateMarketplaceAccountAsync(marketplaceAccountUpdateDto);

            if (!updateMarketplaceAccountResult.Success)
            {
                await ReloadStatusFieldsAsync(marketplaceAccountUpdateViewModel);
                return new ErrorResult(updateMarketplaceAccountResult.Message);
            }

            _marketplaceJobService.DispatchImmediateSingleAccountDataSync(marketplaceAccountUpdateViewModel.Id);

            return new SuccessResult(updateMarketplaceAccountResult.Message);
        }

        public async Task<IResult> DeleteMarketplaceAccountAsync(int marketplaceAccountId)
        {
            return await _marketplaceAccountService.DeleteMarketplaceAccountByIdAsync(marketplaceAccountId);
        }

        private async Task ReloadStatusFieldsAsync(MarketplaceAccountUpdateViewModel marketplaceAccountUpdateViewModel)
        {
            IDataResult<MarketplaceAccountDetailsDto> getMarketplaceAccountResult = await _marketplaceAccountService.GetMarketplaceAccountByIdAsync(marketplaceAccountUpdateViewModel.Id);

            if (!getMarketplaceAccountResult.Success || getMarketplaceAccountResult.Data is null)
                return;

            marketplaceAccountUpdateViewModel.ConnectionState = getMarketplaceAccountResult.Data.ConnectionState;
            marketplaceAccountUpdateViewModel.SyncState = getMarketplaceAccountResult.Data.SyncState;
            marketplaceAccountUpdateViewModel.LastErrorMessage = getMarketplaceAccountResult.Data.LastErrorMessage;
        }
    }
}
