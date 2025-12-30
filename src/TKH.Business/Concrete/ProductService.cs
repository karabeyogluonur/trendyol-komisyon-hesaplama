using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TKH.Business.Abstract;
using TKH.Business.Dtos.Product;
using TKH.Core.Contexts;
using TKH.Core.DataAccess;
using TKH.Core.Utilities.Paging;
using TKH.Core.Utilities.Results;
using TKH.DataAccess.Extensions;
using TKH.Entities;

namespace TKH.Business.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IWorkContext workContext)
        {
            _unitOfWork = unitOfWork;
            _productRepository = _unitOfWork.GetRepository<Product>();
            _mapper = mapper;
            _workContext = workContext;
        }
        public async Task<IDataResult<IPagedList<ProductSummaryDto>>> GetPagedListAsync(ProductListFilterDto productListFilterDto)
        {
            IQueryable<Product> query = _productRepository.GetAll();

            if (!string.IsNullOrEmpty(productListFilterDto.Barcode))
                query = query.Where(p => p.Barcode.Contains(productListFilterDto.Barcode));

            IQueryable<ProductSummaryDto> productSummaryDtos = query.ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider);

            IPagedList<ProductSummaryDto> pagedProductSummaryDto = await productSummaryDtos.ToPagedListAsync(productListFilterDto.PageIndex, productListFilterDto.PageSize);

            return new SuccessDataResult<IPagedList<ProductSummaryDto>>(pagedProductSummaryDto);
        }
    }
}
