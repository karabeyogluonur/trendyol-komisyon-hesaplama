using Microsoft.EntityFrameworkCore;
using TKH.Core.Utilities.Paging;

namespace TKH.DataAccess.Extensions
{
    public static class IQueryablePageListExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }
            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((pageIndex - indexFrom) * pageSize)
                                    .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);
            var pagedList = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                TotalCount = count,
                Items = items,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
            return pagedList;
        }
    }
}
