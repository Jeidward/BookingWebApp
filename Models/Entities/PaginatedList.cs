public sealed class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalItems { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PaginatedList(IReadOnlyList<T> items, int total, int pageIndex, int pageSize)
    {
        Items = items;
        TotalItems = total;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static PaginatedList<T> Create(IReadOnlyList<T> items, int total,
        int pageIndex, int pageSize)
        => new(items, total, pageIndex, pageSize);
}