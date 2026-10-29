namespace SharedKernel.Resources;

/// <summary>
/// A validated value object representing pagination parameters.
/// Use <see cref="Create"/> to construct; it enforces that page number is positive and page size is between 1 and 100.
/// </summary>
public sealed class Page
{
    /// <summary>
    /// Gets the 1-based page number.
    /// </summary>
    public int PageNumber { get; private set; }

    /// <summary>
    /// Gets the number of items per page (1–100).
    /// </summary>
    public int PageSize { get; private set; }

    private Page() { }

    /// <summary>
    /// Creates a validated <see cref="Page"/> instance.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number. Must be greater than 0.</param>
    /// <param name="pageSize">The number of items per page. Must be between 1 and 100.</param>
    /// <returns>A <see cref="Result{Page}"/> containing the page or a validation error.</returns>
    public static Result<Page> Create(int pageNumber, int pageSize)
    {
        return Result.Success((pageNumber, pageSize))
            .Ensure(p => p.pageNumber > 0, Error.InvalidValue($"PageNumber is invalid. Provided value: {pageNumber}"))
            .Ensure(p => p.pageSize is > 0 and <= 100,
                Error.InvalidValue($"PageSize is invalid. Provided value: {pageSize}"))
            .Map(p => new Page
            {
                PageNumber = p.pageNumber,
                PageSize = p.pageSize
            });
    }
}
