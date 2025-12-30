namespace TKH.Core.Utilities.Paging
{
    public interface IPageable
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
    }
}
