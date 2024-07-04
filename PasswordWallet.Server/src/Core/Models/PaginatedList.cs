namespace Core.Models;

public record PaginatedList<T>(
    int PageNumber,
    int PageSize,
    int TotalCount,
    IReadOnlyCollection<T> Items)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}