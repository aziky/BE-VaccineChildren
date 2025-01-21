namespace VaccineChildren.Core.Base;

public class BasePaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; private set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public BasePaginatedList()
    {
        Items = new List<T>();
    }
    public BasePaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize) 
    {
        TotalItems = count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count /(double)pageSize);
        Items = items;
    }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}