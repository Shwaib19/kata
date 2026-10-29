namespace SharedKernel.Resources;

/// <summary>
/// A generic paginated result containing a page of items along with pagination metadata.
/// Use <see cref="Create"/> to construct or <see cref="Blank"/> for an empty result.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Gets the items on the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; private init; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; private init; }

    /// <summary>
    /// Gets the current 1-based page number.
    /// </summary>
    public int Page { get; private init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; private init; }

    private PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    private PagedResult()
    {
        Items = [];
        TotalCount = 0;
        Page = 1;
        PageSize = 0;
    }

    /// <summary>
    /// Creates a paginated result with the specified items and metadata.
    /// </summary>
    /// <param name="items">The items on the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="page">The current 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, page, pageSize);
    }

    /// <summary>
    /// Gets an empty paginated result with no items, page 1, and zero totals.
    /// </summary>
    public static PagedResult<T> Blank => new()
    {
        Items = [],
        TotalCount = 0,
        Page = 1,
        PageSize = 0
    };
}
