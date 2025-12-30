using TKH.Core.Common.Constants;

namespace TKH.Core.Utilities.Paging
{
    public abstract class PageRequest : IPageable
    {
        public int PageIndex { get; set; } = 1;

        private int _pageSize = ApplicationDefaults.DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > ApplicationDefaults.DefaultMaxPageSize ? ApplicationDefaults.DefaultMaxPageSize : value;
        }
    }
}
