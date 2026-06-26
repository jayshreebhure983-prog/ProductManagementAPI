namespace ProductManagement.Application.DTOs;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize > 0
            ? (int)Math.Ceiling(TotalCount / (double)PageSize)
            : 0;
}
