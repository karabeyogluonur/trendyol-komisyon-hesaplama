using AutoMapper;
using TKH.Core.Utilities.Paging;

namespace TKH.Business.Common.Mappings
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap(typeof(IPagedList<>), typeof(IPagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        }
    }
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, IPagedList<TDestination>>
    {
        public IPagedList<TDestination> Convert(IPagedList<TSource> source, IPagedList<TDestination> destination, ResolutionContext context)
        {
            IList<TSource> sourceItems = source.Items;
            IList<TDestination> mappedItems = context.Mapper.Map<IList<TDestination>>(sourceItems);

            PagedList<TDestination> pagedResult = new PagedList<TDestination>
            {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                IndexFrom = source.IndexFrom,
                TotalCount = source.TotalCount,
                TotalPages = source.TotalPages,
                Items = mappedItems
            };

            return pagedResult;
        }
    }
}
