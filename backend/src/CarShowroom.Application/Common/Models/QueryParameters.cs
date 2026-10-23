namespace CarShowroom.Application.Common.Models;

/// <summary>
/// Global paging / ordering / search parameters (adapted from Shared/QueryParameters).
/// </summary>
public class QueryParameters
{
    /// <summary>Set The Maximum Page Size</summary>
    private const int MAX_PAGE_SIZE = 10000;

    /// <summary>Set the default Page size 10</summary>
    private int _pageSize = 10;
    private int _page;

    /// <summary>Get or Set Page Number (0-based)</summary>
    public int Page
    {
        get => _page;
        set => _page = value < 0 ? 0 : value;
    }

    /// <summary>Set or get the PageSize</summary>
    public int Limit
    {
        get => _pageSize;
        set => _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value < 1 ? 1 : value;
    }

    public int Skip => Page * Limit;

    /// <summary>Set or get the OrderName (property name of the returned DTO)</summary>
    public string? OrderName { get; set; }

    /// <summary>Set or get the OrderDirection (asc | desc)</summary>
    public string? OrderDirection { get; set; }

    /// <summary>Free text search</summary>
    public string? Search { get; set; }

    public bool IsDescending => string.Equals(OrderDirection, "desc", StringComparison.OrdinalIgnoreCase);
}
